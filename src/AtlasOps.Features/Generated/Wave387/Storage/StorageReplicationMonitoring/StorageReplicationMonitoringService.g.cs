namespace AtlasOps.Features.Storage.StorageReplicationMonitoring;

using AtlasOps.Features;

public sealed class StorageReplicationMonitoringService(
    IAtlasOpsCapabilityRepository<StorageReplicationMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageReplicationMonitoringValidator validator = new();
    private readonly StorageReplicationMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageReplicationMonitoringChanged>> ExecuteAsync(
        UpdateStorageReplicationMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageReplicationMonitoringChanged>.Invalid(issues);
        }

        StorageReplicationMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageReplicationMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageReplicationMonitoringChanged>.Invalid(
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

        StorageReplicationMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageReplicationMonitoringChanged>.Success(changed);
    }
}