namespace AtlasOps.Features.Storage.BlockVolumeMonitoring;

using AtlasOps.Features;

public sealed class BlockVolumeMonitoringService(
    IAtlasOpsCapabilityRepository<BlockVolumeMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly BlockVolumeMonitoringValidator validator = new();
    private readonly BlockVolumeMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BlockVolumeMonitoringChanged>> ExecuteAsync(
        UpdateBlockVolumeMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BlockVolumeMonitoringChanged>.Invalid(issues);
        }

        BlockVolumeMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BlockVolumeMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BlockVolumeMonitoringChanged>.Invalid(
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

        BlockVolumeMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BlockVolumeMonitoringChanged>.Success(changed);
    }
}