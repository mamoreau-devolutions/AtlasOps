namespace AtlasOps.Features.Storage.StorageLifecycleMonitoring;

using AtlasOps.Features;

public sealed class StorageLifecycleMonitoringService(
    IAtlasOpsCapabilityRepository<StorageLifecycleMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageLifecycleMonitoringValidator validator = new();
    private readonly StorageLifecycleMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageLifecycleMonitoringChanged>> ExecuteAsync(
        UpdateStorageLifecycleMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageLifecycleMonitoringChanged>.Invalid(issues);
        }

        StorageLifecycleMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageLifecycleMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageLifecycleMonitoringChanged>.Invalid(
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

        StorageLifecycleMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageLifecycleMonitoringChanged>.Success(changed);
    }
}