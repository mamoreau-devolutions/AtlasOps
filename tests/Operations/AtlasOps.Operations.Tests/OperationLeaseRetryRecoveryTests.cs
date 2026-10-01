namespace AtlasOps.Operations.Tests;

using AtlasOps.Operations.Contracts;
using AtlasOps.Operations.Runtime;

[TestClass]
public sealed class OperationLeaseRetryRecoveryTests
{
    [TestMethod]
    public void TryAcquire_ValidatesIdentityOwnerAndInclusiveDurationBoundaries()
    {
        OperationLeaseManager manager = new();

        LeaseAcquisitionResult empty = manager.TryAcquire(
            Guid.Empty,
            "owner",
            OperationTestData.Now,
            TimeSpan.FromSeconds(1));
        LeaseAcquisitionResult owner = manager.TryAcquire(
            Guid.NewGuid(),
            " ",
            OperationTestData.Now,
            TimeSpan.FromSeconds(1));
        LeaseAcquisitionResult shortDuration = manager.TryAcquire(
            Guid.NewGuid(),
            "owner",
            OperationTestData.Now,
            TimeSpan.FromSeconds(1).Subtract(TimeSpan.FromTicks(1)));
        LeaseAcquisitionResult boundary = manager.TryAcquire(
            Guid.NewGuid(),
            "owner",
            OperationTestData.Now,
            TimeSpan.FromHours(1));
        LeaseAcquisitionResult longDuration = manager.TryAcquire(
            Guid.NewGuid(),
            "owner",
            OperationTestData.Now,
            TimeSpan.FromHours(1).Add(TimeSpan.FromTicks(1)));

        Assert.AreEqual("Job ID cannot be empty.", empty.Diagnostic);
        Assert.AreEqual("Lease owner is required.", owner.Diagnostic);
        Assert.AreEqual("Lease duration must be between one second and one hour.", shortDuration.Diagnostic);
        Assert.IsTrue(boundary.Acquired);
        Assert.AreEqual(OperationTestData.Now.AddHours(1), boundary.Lease!.ExpiresAt);
        Assert.AreEqual("Lease duration must be between one second and one hour.", longDuration.Diagnostic);
    }

    [TestMethod]
    public void LeaseLifecycle_RejectsActiveAndStaleFencingThenAllowsExpiredTakeover()
    {
        OperationLeaseManager manager = new();
        Guid jobId = Guid.NewGuid();
        LeaseAcquisitionResult first = manager.TryAcquire(
            jobId,
            "owner-1",
            OperationTestData.Now,
            TimeSpan.FromMinutes(1));
        LeaseAcquisitionResult active = manager.TryAcquire(
            jobId,
            "owner-2",
            OperationTestData.Now.AddSeconds(59),
            TimeSpan.FromMinutes(1));
        LeaseAcquisitionResult takeover = manager.TryAcquire(
            jobId,
            "owner-2",
            OperationTestData.Now.AddMinutes(1),
            TimeSpan.FromMinutes(1));
        LeaseAcquisitionResult stale = manager.TryRenew(
            jobId,
            "owner-1",
            first.Lease!.FencingToken,
            OperationTestData.Now.AddMinutes(1),
            TimeSpan.FromMinutes(1));

        Assert.IsFalse(active.Acquired);
        Assert.AreSame(first.Lease, active.Lease);
        Assert.IsTrue(takeover.Acquired);
        Assert.IsGreaterThan(first.Lease.FencingToken, takeover.Lease!.FencingToken);
        Assert.AreEqual("The lease owner or fencing token is stale.", stale.Diagnostic);
        Assert.IsFalse(manager.Release(jobId, "owner-1", first.Lease.FencingToken));
        Assert.IsTrue(manager.Release(jobId, "owner-2", takeover.Lease.FencingToken));
    }

    [TestMethod]
    public void TryRenew_MissingOrExpiredLease_IsRejected()
    {
        OperationLeaseManager manager = new();
        Guid jobId = Guid.NewGuid();
        LeaseAcquisitionResult missing = manager.TryRenew(
            jobId,
            "owner",
            1,
            OperationTestData.Now,
            TimeSpan.FromMinutes(1));
        LeaseAcquisitionResult acquired = manager.TryAcquire(
            jobId,
            "owner",
            OperationTestData.Now,
            TimeSpan.FromSeconds(1));
        LeaseAcquisitionResult expired = manager.TryRenew(
            jobId,
            "owner",
            acquired.Lease!.FencingToken,
            OperationTestData.Now.AddSeconds(1),
            TimeSpan.FromMinutes(1));

        Assert.AreEqual("The lease does not exist.", missing.Diagnostic);
        Assert.AreEqual("The lease has expired.", expired.Diagnostic);
        Assert.AreEqual(OperationTestData.Now.AddSeconds(1), expired.Lease!.ExpiresAt);
    }

    [TestMethod]
    public void TryRenew_ValidOwnerAndFencingToken_ExtendsLeaseWithoutChangingToken()
    {
        OperationLeaseManager manager = new();
        Guid jobId = Guid.NewGuid();
        LeaseAcquisitionResult acquired = manager.TryAcquire(
            jobId,
            "owner",
            OperationTestData.Now,
            TimeSpan.FromMinutes(1));

        LeaseAcquisitionResult renewed = manager.TryRenew(
            jobId,
            "owner",
            acquired.Lease!.FencingToken,
            OperationTestData.Now.AddSeconds(30),
            TimeSpan.FromMinutes(2));

        Assert.IsTrue(renewed.Acquired);
        Assert.AreEqual(acquired.Lease.FencingToken, renewed.Lease!.FencingToken);
        Assert.AreEqual(OperationTestData.Now.AddMinutes(2).AddSeconds(30), renewed.Lease.ExpiresAt);
    }

    [TestMethod]
    public void RetryPlanner_SuccessPermanentAndExhaustedFailures_DoNotRetry()
    {
        OperationRetryPlanner planner = new(TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(1));
        DurableJob job = OperationTestData.Job(attempt: 3, maximumAttempts: 3);

        RetryDecision success = planner.Plan(job, OperationTestData.Result(true), OperationTestData.Now, null);
        RetryDecision permanent = planner.Plan(
            job with { Attempt = 1 },
            OperationTestData.Result(false, OperationFailureKind.Permanent, "permanent"),
            OperationTestData.Now,
            null);
        RetryDecision exhausted = planner.Plan(
            job,
            OperationTestData.Result(false, OperationFailureKind.Transient, "transient"),
            OperationTestData.Now,
            null);

        Assert.IsFalse(success.Retry);
        Assert.IsFalse(success.DeadLetter);
        Assert.IsTrue(permanent.DeadLetter);
        Assert.AreEqual("permanent", permanent.Diagnostic);
        Assert.IsTrue(exhausted.DeadLetter);
        Assert.AreEqual("Maximum attempts have been exhausted.", exhausted.Diagnostic);
    }

    [TestMethod]
    public void RetryPlanner_UsesExponentialRetryAfterAndMaximumCap()
    {
        OperationRetryPlanner planner = new(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10));
        OperationExecutionResult transient = OperationTestData.Result(
            false,
            OperationFailureKind.Transient,
            "retry");

        RetryDecision exponential = planner.Plan(
            OperationTestData.Job(attempt: 2),
            transient,
            OperationTestData.Now,
            null);
        RetryDecision retryAfter = planner.Plan(
            OperationTestData.Job(attempt: 1),
            transient,
            OperationTestData.Now,
            TimeSpan.FromSeconds(8));
        RetryDecision capped = planner.Plan(
            OperationTestData.Job(attempt: 1),
            transient,
            OperationTestData.Now,
            TimeSpan.FromHours(1));

        Assert.AreEqual(OperationTestData.Now.AddSeconds(4), exponential.AvailableAt);
        Assert.AreEqual(OperationTestData.Now.AddSeconds(8), retryAfter.AvailableAt);
        Assert.AreEqual(OperationTestData.Now.AddSeconds(10), capped.AvailableAt);
        Assert.IsTrue(capped.Retry);
        Assert.IsFalse(capped.DeadLetter);
    }

    [TestMethod]
    public void RecoveryPlanner_ProducesOnlyActionableItemsInPriorityThenResourceOrder()
    {
        RecoveryPlanner planner = new();
        Guid later = Guid.Parse("00000000-0000-0000-0000-000000000002");
        Guid earlier = Guid.Parse("00000000-0000-0000-0000-000000000001");
        OperationLease expired = new(later, "owner", OperationTestData.Now, OperationTestData.Now, 1);
        DurableJob retry = OperationTestData.Job(
            DurableJobStatus.WaitingForRetry,
            id: earlier,
            availableAt: OperationTestData.Now);
        DurableJob dead = OperationTestData.Job(
            DurableJobStatus.Failed,
            attempt: 3,
            maximumAttempts: 3,
            id: later);
        OutboxMessage outbox = new(earlier, "queue", "text/plain", [1], OperationTestData.Now, null, 0, null);
        SynchronizationConflict conflict = new(
            later,
            "provider",
            "resource",
            "1",
            "local",
            "remote",
            OperationTestData.Now,
            new Dictionary<string, string>(),
            new Dictionary<string, string>());

        RecoveryPlan plan = planner.Create(
            OperationTestData.Now,
            [dead, retry],
            [expired],
            [outbox],
            [conflict]);

        CollectionAssert.AreEqual(
            new[]
            {
                RecoveryActionKind.ReleaseLease,
                RecoveryActionKind.Retry,
                RecoveryActionKind.DeadLetter,
                RecoveryActionKind.ResumeOutbox,
                RecoveryActionKind.ReviewConflict,
            },
            plan.Actions.Select(static action => action.Kind).ToArray());
        Assert.AreEqual(OperationTestData.Now, plan.CreatedAt);
        CollectionAssert.AreEqual(new[] { 10, 20, 30, 40, 50 }, plan.Actions.Select(static action => action.Priority).ToArray());
    }

    [TestMethod]
    public void RecoveryPlanner_NonActionableItems_ReturnEmptyPlan()
    {
        RecoveryPlanner planner = new();
        OperationLease active = new(
            Guid.NewGuid(),
            "owner",
            OperationTestData.Now,
            OperationTestData.Now.AddMinutes(1),
            1);
        DurableJob future = OperationTestData.Job(
            DurableJobStatus.WaitingForRetry,
            availableAt: OperationTestData.Now.AddMinutes(1));
        OutboxMessage published = new(
            Guid.NewGuid(),
            "queue",
            "text/plain",
            [1],
            OperationTestData.Now,
            OperationTestData.Now,
            0,
            null);

        RecoveryPlan plan = planner.Create(OperationTestData.Now, [future], [active], [published], []);

        Assert.IsEmpty(plan.Actions);
    }
}
