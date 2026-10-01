namespace AtlasOps.Operations.Runtime;

using AtlasOps.Operations.Contracts;

public sealed class InMemoryDurableJobStore : IDurableJobStore
{
    private readonly object gate = new();
    private readonly Dictionary<Guid, DurableJob> jobs = [];

    public ValueTask<JobMutationResult> EnqueueAsync(
        DurableJob job,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (this.gate)
        {
            if (job.Id == Guid.Empty)
            {
                return ValueTask.FromResult(
                    new JobMutationResult(false, "Job ID cannot be empty.", null));
            }

            if (job.Revision != 0)
            {
                return ValueTask.FromResult(
                    new JobMutationResult(false, "New jobs must have revision zero.", null));
            }

            if (!this.jobs.TryAdd(job.Id, job))
            {
                return ValueTask.FromResult(
                    new JobMutationResult(false, "A job with the same ID already exists.", this.jobs[job.Id]));
            }

            return ValueTask.FromResult(new JobMutationResult(true, string.Empty, job));
        }
    }

    public ValueTask<IReadOnlyList<DurableJob>> GetAvailableAsync(
        DateTimeOffset now,
        int maximumCount,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (maximumCount < 1)
        {
            return ValueTask.FromResult<IReadOnlyList<DurableJob>>([]);
        }

        lock (this.gate)
        {
            DurableJob[] available = this.jobs.Values
                .Where(job =>
                    job.AvailableAt <= now &&
                    job.Status is DurableJobStatus.Pending or DurableJobStatus.WaitingForRetry)
                .OrderBy(static job => job.AvailableAt)
                .ThenBy(static job => job.CreatedAt)
                .ThenBy(static job => job.Id)
                .Take(maximumCount)
                .ToArray();

            return ValueTask.FromResult<IReadOnlyList<DurableJob>>(available);
        }
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

        lock (this.gate)
        {
            if (!this.jobs.TryGetValue(jobId, out DurableJob? job))
            {
                return ValueTask.FromResult(
                    new JobMutationResult(false, "The job does not exist.", null));
            }

            if (job.Revision != expectedRevision)
            {
                return ValueTask.FromResult(
                    new JobMutationResult(false, "The job revision is stale.", job));
            }

            JobMutationResult result = OperationStateMachine.Transition(
                job,
                nextStatus,
                DateTimeOffset.UtcNow,
                availableAt,
                diagnostic);

            if (result.Succeeded && result.Job is not null)
            {
                this.jobs[jobId] = result.Job;
            }

            return ValueTask.FromResult(result);
        }
    }
}
