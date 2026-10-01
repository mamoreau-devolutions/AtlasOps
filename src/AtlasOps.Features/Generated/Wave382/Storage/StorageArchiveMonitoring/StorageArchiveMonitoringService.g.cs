namespace AtlasOps.Features.Storage.StorageArchiveMonitoring;

using AtlasOps.Features;

public sealed class StorageArchiveMonitoringService(
    IAtlasOpsCapabilityRepository<StorageArchiveMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageArchiveMonitoringValidator validator = new();
    private readonly StorageArchiveMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageArchiveMonitoringChanged>> ExecuteAsync(
        UpdateStorageArchiveMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageArchiveMonitoringChanged>.Invalid(issues);
        }

        StorageArchiveMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageArchiveMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageArchiveMonitoringChanged>.Invalid(
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

        StorageArchiveMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageArchiveMonitoringChanged>.Success(changed);
    }
}