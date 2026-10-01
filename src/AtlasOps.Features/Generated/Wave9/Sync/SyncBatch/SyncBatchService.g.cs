namespace AtlasOps.Features.Sync.SyncBatch;

using AtlasOps.Features;

public sealed class SyncBatchService(
    IAtlasOpsCapabilityRepository<SyncBatchItem> repository,
    TimeProvider timeProvider)
{
    private readonly SyncBatchValidator validator = new();
    private readonly SyncBatchPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SyncBatchChanged>> ExecuteAsync(
        UpdateSyncBatchCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SyncBatchChanged>.Invalid(issues);
        }

        SyncBatchItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SyncBatchItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SyncBatchChanged>.Invalid(
            [
                new("State", $"Cannot transition from '{previousState}' to '{command.TargetState}'."),
            ]);
        }

        entity.Name = command.Name.Trim();
        entity.Owner = command.Owner.Trim();
        entity.State = command.TargetState;
        entity.Priority = command.Priority;
        entity.IsEnabled = command.IsEnabled;
        DateTimeOffset now = timeProvider.GetUtcNow();
        entity.MarkUpdated(now);
        await repository.SaveAsync(entity, cancellationToken);

        SyncBatchChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SyncBatchChanged>.Success(changed);
    }
}