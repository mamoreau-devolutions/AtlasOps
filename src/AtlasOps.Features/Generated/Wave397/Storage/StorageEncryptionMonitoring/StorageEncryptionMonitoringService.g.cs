namespace AtlasOps.Features.Storage.StorageEncryptionMonitoring;

using AtlasOps.Features;

public sealed class StorageEncryptionMonitoringService(
    IAtlasOpsCapabilityRepository<StorageEncryptionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageEncryptionMonitoringValidator validator = new();
    private readonly StorageEncryptionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageEncryptionMonitoringChanged>> ExecuteAsync(
        UpdateStorageEncryptionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageEncryptionMonitoringChanged>.Invalid(issues);
        }

        StorageEncryptionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageEncryptionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageEncryptionMonitoringChanged>.Invalid(
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

        StorageEncryptionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageEncryptionMonitoringChanged>.Success(changed);
    }
}