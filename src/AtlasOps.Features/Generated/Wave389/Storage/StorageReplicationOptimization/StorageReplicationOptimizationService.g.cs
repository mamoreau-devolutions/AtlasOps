namespace AtlasOps.Features.Storage.StorageReplicationOptimization;

using AtlasOps.Features;

public sealed class StorageReplicationOptimizationService(
    IAtlasOpsCapabilityRepository<StorageReplicationOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageReplicationOptimizationValidator validator = new();
    private readonly StorageReplicationOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageReplicationOptimizationChanged>> ExecuteAsync(
        UpdateStorageReplicationOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageReplicationOptimizationChanged>.Invalid(issues);
        }

        StorageReplicationOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageReplicationOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageReplicationOptimizationChanged>.Invalid(
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

        StorageReplicationOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageReplicationOptimizationChanged>.Success(changed);
    }
}