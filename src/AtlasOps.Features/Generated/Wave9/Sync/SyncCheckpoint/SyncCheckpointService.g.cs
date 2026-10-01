namespace AtlasOps.Features.Sync.SyncCheckpoint;

using AtlasOps.Features;

public sealed class SyncCheckpointService(
    IAtlasOpsCapabilityRepository<SyncCheckpointItem> repository,
    TimeProvider timeProvider)
{
    private readonly SyncCheckpointValidator validator = new();
    private readonly SyncCheckpointPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SyncCheckpointChanged>> ExecuteAsync(
        UpdateSyncCheckpointCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SyncCheckpointChanged>.Invalid(issues);
        }

        SyncCheckpointItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SyncCheckpointItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SyncCheckpointChanged>.Invalid(
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

        SyncCheckpointChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SyncCheckpointChanged>.Success(changed);
    }
}