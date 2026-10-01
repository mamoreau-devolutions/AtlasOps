namespace AtlasOps.Operations.Runtime;

using AtlasOps.Operations.Contracts;

public static class OperationStateMachine
{
    private static readonly IReadOnlyDictionary<DurableJobStatus, IReadOnlySet<DurableJobStatus>> Transitions =
        new Dictionary<DurableJobStatus, IReadOnlySet<DurableJobStatus>>
        {
            [DurableJobStatus.Pending] = new HashSet<DurableJobStatus>
            {
                DurableJobStatus.Leased,
                DurableJobStatus.Cancelled,
            },
            [DurableJobStatus.Leased] = new HashSet<DurableJobStatus>
            {
                DurableJobStatus.Running,
                DurableJobStatus.Pending,
                DurableJobStatus.Cancelled,
            },
            [DurableJobStatus.Running] = new HashSet<DurableJobStatus>
            {
                DurableJobStatus.Succeeded,
                DurableJobStatus.WaitingForRetry,
                DurableJobStatus.Failed,
                DurableJobStatus.Cancelled,
                DurableJobStatus.DeadLettered,
            },
            [DurableJobStatus.WaitingForRetry] = new HashSet<DurableJobStatus>
            {
                DurableJobStatus.Leased,
                DurableJobStatus.Cancelled,
                DurableJobStatus.DeadLettered,
            },
            [DurableJobStatus.Failed] = new HashSet<DurableJobStatus>
            {
                DurableJobStatus.WaitingForRetry,
                DurableJobStatus.DeadLettered,
            },
            [DurableJobStatus.Succeeded] = new HashSet<DurableJobStatus>(),
            [DurableJobStatus.Cancelled] = new HashSet<DurableJobStatus>(),
            [DurableJobStatus.DeadLettered] = new HashSet<DurableJobStatus>(),
        };

    public static bool CanTransition(DurableJobStatus current, DurableJobStatus next)
    {
        return Transitions.TryGetValue(current, out IReadOnlySet<DurableJobStatus>? targets) &&
               targets.Contains(next);
    }

    public static JobMutationResult Transition(
        DurableJob job,
        DurableJobStatus next,
        DateTimeOffset now,
        DateTimeOffset availableAt,
        string? diagnostic)
    {
        if (!CanTransition(job.Status, next))
        {
            return new JobMutationResult(
                false,
                $"Transition from {job.Status} to {next} is not allowed.",
                job);
        }

        int attempt = next == DurableJobStatus.Running ? job.Attempt + 1 : job.Attempt;
        if (attempt > job.MaximumAttempts)
        {
            return new JobMutationResult(false, "Maximum attempts have been exhausted.", job);
        }

        DurableJob updated = job with
        {
            Status = next,
            Attempt = attempt,
            AvailableAt = availableAt,
            UpdatedAt = now,
            Revision = job.Revision + 1,
            LastDiagnostic = diagnostic,
        };

        return new JobMutationResult(true, string.Empty, updated);
    }
}
