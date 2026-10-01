namespace AtlasOps.Features.Storage.StorageTransferMonitoring;

using AtlasOps.Features;

public sealed class StorageTransferMonitoringService(
    IAtlasOpsCapabilityRepository<StorageTransferMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageTransferMonitoringValidator validator = new();
    private readonly StorageTransferMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageTransferMonitoringChanged>> ExecuteAsync(
        UpdateStorageTransferMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageTransferMonitoringChanged>.Invalid(issues);
        }

        StorageTransferMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageTransferMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageTransferMonitoringChanged>.Invalid(
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

        StorageTransferMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageTransferMonitoringChanged>.Success(changed);
    }
}