namespace AtlasOps.Features.Storage.StorageQuotaMonitoring;

using AtlasOps.Features;

public sealed class StorageQuotaMonitoringService(
    IAtlasOpsCapabilityRepository<StorageQuotaMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageQuotaMonitoringValidator validator = new();
    private readonly StorageQuotaMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageQuotaMonitoringChanged>> ExecuteAsync(
        UpdateStorageQuotaMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageQuotaMonitoringChanged>.Invalid(issues);
        }

        StorageQuotaMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageQuotaMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageQuotaMonitoringChanged>.Invalid(
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

        StorageQuotaMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageQuotaMonitoringChanged>.Success(changed);
    }
}