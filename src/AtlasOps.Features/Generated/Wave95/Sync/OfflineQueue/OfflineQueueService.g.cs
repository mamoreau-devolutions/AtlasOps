namespace AtlasOps.Features.Sync.OfflineQueue;

using AtlasOps.Features;

public sealed class OfflineQueueService(
    IAtlasOpsCapabilityRepository<OfflineQueueItem> repository,
    TimeProvider timeProvider)
{
    private readonly OfflineQueueValidator validator = new();
    private readonly OfflineQueuePolicy policy = new();

    public async Task<AtlasOpsOperationResult<OfflineQueueChanged>> ExecuteAsync(
        UpdateOfflineQueueCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<OfflineQueueChanged>.Invalid(issues);
        }

        OfflineQueueItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new OfflineQueueItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<OfflineQueueChanged>.Invalid(
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

        OfflineQueueChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<OfflineQueueChanged>.Success(changed);
    }
}