namespace AtlasOps.Operations.Runtime;

using AtlasOps.Operations.Contracts;

public sealed class OperationCoordinator
{
    private readonly IReadOnlyDictionary<string, IOperationHandler> handlers;
    private readonly IDurableJobStore jobStore;
    private readonly OperationLeaseManager leaseManager;
    private readonly OperationRetryPlanner retryPlanner;

    public OperationCoordinator(
        IEnumerable<IOperationHandler> handlers,
        IDurableJobStore jobStore,
        OperationLeaseManager leaseManager,
        OperationRetryPlanner retryPlanner)
    {
        this.handlers = handlers.ToDictionary(
            static handler => handler.OperationId,
            StringComparer.OrdinalIgnoreCase);
        this.jobStore = jobStore;
        this.leaseManager = leaseManager;
        this.retryPlanner = retryPlanner;
    }

    public async ValueTask<OperationExecutionResult> ExecuteNextAsync(
        string owner,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<DurableJob> available = await this.jobStore.GetAvailableAsync(
            now,
            1,
            cancellationToken);
        if (available.Count == 0)
        {
            return Failure(Guid.Empty, OperationFailureKind.Validation, "No job is available.");
        }

        DurableJob job = available[0];
        LeaseAcquisitionResult acquisition = this.leaseManager.TryAcquire(
            job.Id,
            owner,
            now,
            TimeSpan.FromMinutes(5));
        if (!acquisition.Acquired || acquisition.Lease is null)
        {
            return Failure(job.Envelope.Id, OperationFailureKind.Conflict, acquisition.Diagnostic);
        }

        JobMutationResult leased = await this.jobStore.MutateAsync(
            job.Id,
            job.Revision,
            DurableJobStatus.Leased,
            now,
            null,
            cancellationToken);
        if (!leased.Succeeded || leased.Job is null)
        {
            this.leaseManager.Release(job.Id, owner, acquisition.Lease.FencingToken);
            return Failure(job.Envelope.Id, OperationFailureKind.Conflict, leased.Diagnostic);
        }

        JobMutationResult running = await this.jobStore.MutateAsync(
            job.Id,
            leased.Job.Revision,
            DurableJobStatus.Running,
            now,
            null,
            cancellationToken);
        if (!running.Succeeded || running.Job is null)
        {
            this.leaseManager.Release(job.Id, owner, acquisition.Lease.FencingToken);
            return Failure(job.Envelope.Id, OperationFailureKind.Conflict, running.Diagnostic);
        }

        OperationExecutionResult result;
        if (!this.handlers.TryGetValue(job.Envelope.Operation, out IOperationHandler? handler))
        {
            result = Failure(
                job.Envelope.Id,
                OperationFailureKind.Validation,
                $"Operation '{job.Envelope.Operation}' is not registered.");
        }
        else
        {
            result = await handler.ExecuteAsync(job.Envelope, cancellationToken);
        }

        await this.CompleteAsync(running.Job, result, now, cancellationToken);
        this.leaseManager.Release(job.Id, owner, acquisition.Lease.FencingToken);
        return result;
    }

    private async ValueTask CompleteAsync(
        DurableJob job,
        OperationExecutionResult result,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (result.Succeeded)
        {
            await this.jobStore.MutateAsync(
                job.Id,
                job.Revision,
                DurableJobStatus.Succeeded,
                now,
                null,
                cancellationToken);
            return;
        }

        RetryDecision retry = this.retryPlanner.Plan(job, result, now, null);
        DurableJobStatus next = retry.Retry
            ? DurableJobStatus.WaitingForRetry
            : DurableJobStatus.DeadLettered;
        await this.jobStore.MutateAsync(
            job.Id,
            job.Revision,
            next,
            retry.AvailableAt,
            retry.Diagnostic,
            cancellationToken);
    }

    private static OperationExecutionResult Failure(
        Guid operationId,
        OperationFailureKind kind,
        string diagnostic)
    {
        return new OperationExecutionResult(
            operationId,
            false,
            kind,
            diagnostic,
            null,
            OperationContract.EmptyDetails);
    }
}
