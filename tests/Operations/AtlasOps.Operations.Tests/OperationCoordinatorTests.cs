namespace AtlasOps.Operations.Tests;

using AtlasOps.Operations.Contracts;
using AtlasOps.Operations.Runtime;

[TestClass]
public sealed class OperationCoordinatorTests
{
    [TestMethod]
    public async Task ExecuteNextAsync_NoAvailableJob_ReturnsValidationFailure()
    {
        RecordingJobStore store = new(null);
        OperationCoordinator coordinator = CreateCoordinator([], store);

        OperationExecutionResult result = await coordinator.ExecuteNextAsync(
            "owner",
            OperationTestData.Now,
            CancellationToken.None);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(OperationFailureKind.Validation, result.FailureKind);
        Assert.AreEqual("No job is available.", result.Diagnostic);
        Assert.AreEqual(Guid.Empty, result.OperationId);
    }

    [TestMethod]
    public async Task ExecuteNextAsync_CaseInsensitiveHandlerSuccess_TransitionsToSucceeded()
    {
        DurableJob job = OperationTestData.Job(operation: "SYNC");
        RecordingJobStore store = new(job);
        StubHandler handler = new("sync", OperationTestData.Result(true));
        OperationCoordinator coordinator = CreateCoordinator([handler], store);

        OperationExecutionResult result = await coordinator.ExecuteNextAsync(
            "owner",
            OperationTestData.Now,
            CancellationToken.None);

        Assert.IsTrue(result.Succeeded);
        CollectionAssert.AreEqual(
            new[] { DurableJobStatus.Leased, DurableJobStatus.Running, DurableJobStatus.Succeeded },
            store.Mutations.ToArray());
        Assert.AreEqual(1, handler.ExecutionCount);
    }

    [TestMethod]
    public async Task ExecuteNextAsync_UnregisteredHandler_DeadLettersValidationFailure()
    {
        RecordingJobStore store = new(OperationTestData.Job(operation: "missing"));
        OperationCoordinator coordinator = CreateCoordinator([], store);

        OperationExecutionResult result = await coordinator.ExecuteNextAsync(
            "owner",
            OperationTestData.Now,
            CancellationToken.None);

        Assert.AreEqual(OperationFailureKind.Validation, result.FailureKind);
        StringAssert.Contains(result.Diagnostic, "is not registered");
        Assert.AreEqual(DurableJobStatus.DeadLettered, store.Mutations[^1]);
    }

    [TestMethod]
    public async Task ExecuteNextAsync_TransientFailure_SchedulesRetry()
    {
        RecordingJobStore store = new(OperationTestData.Job());
        StubHandler handler = new(
            "sync",
            OperationTestData.Result(false, OperationFailureKind.Transient, "try again"));
        OperationCoordinator coordinator = CreateCoordinator([handler], store);

        OperationExecutionResult result = await coordinator.ExecuteNextAsync(
            "owner",
            OperationTestData.Now,
            CancellationToken.None);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(DurableJobStatus.WaitingForRetry, store.Mutations[^1]);
        Assert.AreEqual("try again", store.LastDiagnostic);
        Assert.AreEqual(OperationTestData.Now.AddSeconds(1), store.LastAvailableAt);
    }

    [TestMethod]
    public async Task ExecuteNextAsync_PreCancelledToken_PropagatesCancellation()
    {
        InMemoryDurableJobStore store = new();
        OperationCoordinator coordinator = new(
            [],
            store,
            new OperationLeaseManager(),
            new OperationRetryPlanner(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1)));
        using CancellationTokenSource source = new();
        source.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            async () => await coordinator.ExecuteNextAsync("owner", OperationTestData.Now, source.Token));
    }

    private static OperationCoordinator CreateCoordinator(
        IEnumerable<IOperationHandler> handlers,
        IDurableJobStore store)
    {
        return new OperationCoordinator(
            handlers,
            store,
            new OperationLeaseManager(),
            new OperationRetryPlanner(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1)));
    }

    private sealed class StubHandler : IOperationHandler
    {
        private readonly OperationExecutionResult result;

        public StubHandler(string operationId, OperationExecutionResult result)
        {
            this.OperationId = operationId;
            this.result = result;
        }

        public string OperationId { get; }

        public int ExecutionCount { get; private set; }

        public ValueTask<OperationExecutionResult> ExecuteAsync(
            OperationEnvelope envelope,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ExecutionCount++;
            return ValueTask.FromResult(this.result with { OperationId = envelope.Id });
        }
    }

    private sealed class RecordingJobStore : IDurableJobStore
    {
        private DurableJob? job;

        public RecordingJobStore(DurableJob? job)
        {
            this.job = job;
        }

        public List<DurableJobStatus> Mutations { get; } = [];

        public DateTimeOffset LastAvailableAt { get; private set; }

        public string? LastDiagnostic { get; private set; }

        public ValueTask<JobMutationResult> EnqueueAsync(
            DurableJob job,
            CancellationToken cancellationToken)
        {
            this.job = job;
            return ValueTask.FromResult(new JobMutationResult(true, string.Empty, job));
        }

        public ValueTask<IReadOnlyList<DurableJob>> GetAvailableAsync(
            DateTimeOffset now,
            int maximumCount,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IReadOnlyList<DurableJob> result = this.job is null ? [] : [this.job];
            return ValueTask.FromResult(result);
        }

        public ValueTask<JobMutationResult> MutateAsync(
            Guid jobId,
            long expectedRevision,
            DurableJobStatus nextStatus,
            DateTimeOffset availableAt,
            string? diagnostic,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (this.job is null || this.job.Revision != expectedRevision)
            {
                return ValueTask.FromResult(new JobMutationResult(false, "stale", this.job));
            }

            this.Mutations.Add(nextStatus);
            this.LastAvailableAt = availableAt;
            this.LastDiagnostic = diagnostic;
            this.job = this.job with
            {
                Status = nextStatus,
                Attempt = nextStatus == DurableJobStatus.Running ? this.job.Attempt + 1 : this.job.Attempt,
                Revision = this.job.Revision + 1,
                AvailableAt = availableAt,
                LastDiagnostic = diagnostic,
            };
            return ValueTask.FromResult(new JobMutationResult(true, string.Empty, this.job));
        }
    }
}
