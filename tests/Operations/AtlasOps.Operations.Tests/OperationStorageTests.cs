namespace AtlasOps.Operations.Tests;

using System.Security.Cryptography;

using AtlasOps.Operations.Contracts;
using AtlasOps.Operations.Runtime;

[TestClass]
public sealed class OperationStorageTests
{
    [TestMethod]
    public async Task EnqueueAsync_InvalidRevisionEmptyIdAndDuplicate_AreRejected()
    {
        InMemoryDurableJobStore store = new();
        DurableJob job = OperationTestData.Job();

        JobMutationResult empty = await store.EnqueueAsync(job with { Id = Guid.Empty }, CancellationToken.None);
        JobMutationResult revision = await store.EnqueueAsync(job with { Revision = 1 }, CancellationToken.None);
        JobMutationResult first = await store.EnqueueAsync(job, CancellationToken.None);
        JobMutationResult duplicate = await store.EnqueueAsync(job, CancellationToken.None);

        Assert.AreEqual("Job ID cannot be empty.", empty.Diagnostic);
        Assert.AreEqual("New jobs must have revision zero.", revision.Diagnostic);
        Assert.IsTrue(first.Succeeded);
        Assert.IsFalse(duplicate.Succeeded);
        Assert.AreSame(job, duplicate.Job);
    }

    [TestMethod]
    public async Task GetAvailableAsync_FiltersAndOrdersByAvailabilityCreationAndId()
    {
        InMemoryDurableJobStore store = new();
        Guid firstId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        Guid secondId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        DurableJob second = OperationTestData.Job(id: secondId);
        DurableJob first = OperationTestData.Job(id: firstId);
        DurableJob future = OperationTestData.Job(
            id: Guid.Parse("00000000-0000-0000-0000-000000000003"),
            availableAt: OperationTestData.Now.AddMinutes(1));
        DurableJob terminal = OperationTestData.Job(
            DurableJobStatus.Succeeded,
            id: Guid.Parse("00000000-0000-0000-0000-000000000004"));
        await store.EnqueueAsync(second, CancellationToken.None);
        await store.EnqueueAsync(first, CancellationToken.None);
        await store.EnqueueAsync(future, CancellationToken.None);
        await store.EnqueueAsync(terminal, CancellationToken.None);

        IReadOnlyList<DurableJob> available = await store.GetAvailableAsync(
            OperationTestData.Now,
            2,
            CancellationToken.None);
        IReadOnlyList<DurableJob> none = await store.GetAvailableAsync(
            OperationTestData.Now,
            0,
            CancellationToken.None);

        CollectionAssert.AreEqual(new[] { firstId, secondId }, available.Select(static job => job.Id).ToArray());
        Assert.IsEmpty(none);
    }

    [TestMethod]
    public async Task MutateAsync_MissingStaleAndSuccessfulTransitions_ReturnExpectedRevisionEvidence()
    {
        InMemoryDurableJobStore store = new();
        DurableJob job = OperationTestData.Job();
        await store.EnqueueAsync(job, CancellationToken.None);

        JobMutationResult missing = await store.MutateAsync(
            Guid.NewGuid(),
            0,
            DurableJobStatus.Leased,
            OperationTestData.Now,
            null,
            CancellationToken.None);
        JobMutationResult stale = await store.MutateAsync(
            job.Id,
            1,
            DurableJobStatus.Leased,
            OperationTestData.Now,
            null,
            CancellationToken.None);
        JobMutationResult changed = await store.MutateAsync(
            job.Id,
            0,
            DurableJobStatus.Leased,
            OperationTestData.Now,
            "owner",
            CancellationToken.None);

        Assert.AreEqual("The job does not exist.", missing.Diagnostic);
        Assert.AreEqual("The job revision is stale.", stale.Diagnostic);
        Assert.AreEqual(0L, stale.Job!.Revision);
        Assert.IsTrue(changed.Succeeded);
        Assert.AreEqual(1L, changed.Job!.Revision);
        Assert.AreEqual(DurableJobStatus.Leased, changed.Job.Status);
    }

    [TestMethod]
    public async Task Stores_PreCancelledOperations_ThrowOperationCanceledException()
    {
        InMemoryDurableJobStore jobs = new();
        InMemoryCheckpointStore checkpoints = new();
        using CancellationTokenSource source = new();
        source.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            async () => await jobs.GetAvailableAsync(OperationTestData.Now, 1, source.Token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            async () => await checkpoints.GetAsync("provider", "scope", source.Token));
    }

    [TestMethod]
    public async Task CheckpointWrite_NormalizesKeysAndRejectsStaleAndNonMonotonicRevisions()
    {
        InMemoryCheckpointStore store = new();
        SynchronizationCheckpoint first = new(
            "Provider ",
            " Scope",
            "cursor-1",
            OperationTestData.Now,
            1);

        CheckpointMutationResult written = await store.WriteAsync(first, 0, CancellationToken.None);
        SynchronizationCheckpoint? normalized = await store.GetAsync(
            " provider",
            "scope ",
            CancellationToken.None);
        CheckpointMutationResult stale = await store.WriteAsync(
            first with { Cursor = "cursor-2", Revision = 2 },
            0,
            CancellationToken.None);
        CheckpointMutationResult nonMonotonic = await store.WriteAsync(
            first with { Cursor = "cursor-2", Revision = 3 },
            1,
            CancellationToken.None);

        Assert.IsTrue(written.Succeeded);
        Assert.AreEqual("cursor-1", normalized!.Cursor);
        Assert.AreEqual("The checkpoint revision is stale.", stale.Diagnostic);
        Assert.AreEqual("The next checkpoint revision must be monotonic.", nonMonotonic.Diagnostic);
        Assert.AreEqual(1L, nonMonotonic.Checkpoint!.Revision);
    }

    [TestMethod]
    public async Task CheckpointWrite_MissingRequiredValues_IsRejected()
    {
        InMemoryCheckpointStore store = new();
        SynchronizationCheckpoint checkpoint = new("", "scope", "cursor", OperationTestData.Now, 1);

        CheckpointMutationResult result = await store.WriteAsync(checkpoint, 0, CancellationToken.None);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual("Provider, scope, and cursor are required.", result.Diagnostic);
        Assert.IsNull(result.Checkpoint);
    }

    [TestMethod]
    public void Accept_DuplicateSameContentIsIdempotentButChangedContentConflicts()
    {
        InboxOutboxStore store = new();
        byte[] payload = "payload"u8.ToArray();
        string hash = Convert.ToHexString(SHA256.HashData(payload));
        InboxMessage first = new(" Provider ", "message-1", "text/plain", payload, OperationTestData.Now, hash);

        InboxAcceptanceResult accepted = store.Accept(first);
        InboxAcceptanceResult duplicate = store.Accept(first with { ProviderId = "provider" });
        byte[] changedPayload = "changed"u8.ToArray();
        InboxAcceptanceResult conflict = store.Accept(first with
        {
            Payload = changedPayload,
            ContentHash = Convert.ToHexString(SHA256.HashData(changedPayload)),
        });

        Assert.IsTrue(accepted.Accepted);
        Assert.IsFalse(accepted.Duplicate);
        Assert.IsTrue(duplicate.Accepted);
        Assert.IsTrue(duplicate.Duplicate);
        Assert.IsFalse(conflict.Accepted);
        Assert.IsTrue(conflict.Duplicate);
        Assert.AreEqual("The duplicate message has different content.", conflict.Diagnostic);
    }

    [TestMethod]
    public void Accept_InvalidIdentityOrHash_IsRejected()
    {
        InboxOutboxStore store = new();
        InboxMessage missing = new("", "id", "text/plain", [1], OperationTestData.Now, "00");
        InboxMessage badHash = missing with { ProviderId = "provider" };

        InboxAcceptanceResult missingResult = store.Accept(missing);
        InboxAcceptanceResult hashResult = store.Accept(badHash);

        Assert.AreEqual("Provider and message IDs are required.", missingResult.Diagnostic);
        Assert.AreEqual("The payload hash does not match.", hashResult.Diagnostic);
        Assert.IsFalse(hashResult.Duplicate);
    }

    [TestMethod]
    public void Outbox_OrdersPendingTracksFailuresAndPublishedMessagesBecomeTerminal()
    {
        InboxOutboxStore store = new();
        Guid laterId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        Guid earlierId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        OutboxMessage later = new(laterId, "queue", "text/plain", [2], OperationTestData.Now, null, 0, null);
        OutboxMessage earlier = new(earlierId, "queue", "text/plain", [1], OperationTestData.Now, null, 0, null);

        Assert.IsTrue(store.AddOutbox(later));
        Assert.IsTrue(store.AddOutbox(earlier));
        Assert.IsFalse(store.AddOutbox(earlier));
        CollectionAssert.AreEqual(
            new[] { earlierId, laterId },
            store.GetPending(2).Select(static message => message.Id).ToArray());
        Assert.IsTrue(store.MarkFailed(earlierId, "retry"));
        OutboxMessage failed = store.GetPending(2).Single(message => message.Id == earlierId);
        Assert.AreEqual(1, failed.Attempt);
        Assert.AreEqual("retry", failed.LastDiagnostic);
        Assert.IsTrue(store.MarkPublished(earlierId, OperationTestData.Now.AddMinutes(1)));
        Assert.IsFalse(store.MarkPublished(earlierId, OperationTestData.Now.AddMinutes(2)));
        Assert.IsFalse(store.MarkFailed(earlierId, "late"));
        CollectionAssert.AreEqual(new[] { laterId }, store.GetPending(2).Select(static message => message.Id).ToArray());
    }

    [TestMethod]
    public void AddOutbox_InvalidValuesAndNonPositiveCount_AreRejected()
    {
        InboxOutboxStore store = new();
        OutboxMessage invalid = new(Guid.Empty, "", "text/plain", [], OperationTestData.Now, null, 0, null);

        Assert.IsFalse(store.AddOutbox(invalid));
        Assert.IsEmpty(store.GetPending(0));
        Assert.IsFalse(store.MarkPublished(Guid.NewGuid(), OperationTestData.Now));
    }
}
