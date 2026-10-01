namespace AtlasOps.Operations.Runtime;

using AtlasOps.Operations.Contracts;

public enum RecoveryActionKind
{
    ReleaseLease,
    Retry,
    DeadLetter,
    ResumeOutbox,
    ReviewConflict,
}

public sealed record RecoveryAction(
    RecoveryActionKind Kind,
    string ResourceId,
    int Priority,
    string Reason);

public sealed record RecoveryPlan(
    DateTimeOffset CreatedAt,
    IReadOnlyList<RecoveryAction> Actions);

public sealed class RecoveryPlanner
{
    public RecoveryPlan Create(
        DateTimeOffset now,
        IEnumerable<DurableJob> jobs,
        IEnumerable<OperationLease> leases,
        IEnumerable<OutboxMessage> outbox,
        IEnumerable<SynchronizationConflict> conflicts)
    {
        List<RecoveryAction> actions = [];

        actions.AddRange(
            leases
                .Where(lease => lease.ExpiresAt <= now)
                .Select(static lease => new RecoveryAction(
                    RecoveryActionKind.ReleaseLease,
                    lease.JobId.ToString("D"),
                    10,
                    "Lease expired before completion.")));

        foreach (DurableJob job in jobs)
        {
            if (job.Status == DurableJobStatus.WaitingForRetry && job.AvailableAt <= now)
            {
                actions.Add(new RecoveryAction(
                    RecoveryActionKind.Retry,
                    job.Id.ToString("D"),
                    20,
                    "Retry delay elapsed."));
            }
            else if (job.Status == DurableJobStatus.Failed && job.Attempt >= job.MaximumAttempts)
            {
                actions.Add(new RecoveryAction(
                    RecoveryActionKind.DeadLetter,
                    job.Id.ToString("D"),
                    30,
                    "Maximum attempts exhausted."));
            }
        }

        actions.AddRange(
            outbox
                .Where(static message => message.PublishedAt is null)
                .Select(static message => new RecoveryAction(
                    RecoveryActionKind.ResumeOutbox,
                    message.Id.ToString("D"),
                    40,
                    "Outbox publication is incomplete.")));

        actions.AddRange(
            conflicts.Select(static conflict => new RecoveryAction(
                RecoveryActionKind.ReviewConflict,
                conflict.Id.ToString("D"),
                50,
                "Synchronization conflict requires a resolution policy.")));

        RecoveryAction[] ordered = actions
            .OrderBy(static action => action.Priority)
            .ThenBy(static action => action.ResourceId, StringComparer.Ordinal)
            .ToArray();
        return new RecoveryPlan(now, ordered);
    }
}
