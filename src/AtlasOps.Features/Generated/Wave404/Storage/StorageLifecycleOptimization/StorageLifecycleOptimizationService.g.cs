namespace AtlasOps.Features.Storage.StorageLifecycleOptimization;

using AtlasOps.Features;

public sealed class StorageLifecycleOptimizationService(
    IAtlasOpsCapabilityRepository<StorageLifecycleOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageLifecycleOptimizationValidator validator = new();
    private readonly StorageLifecycleOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageLifecycleOptimizationChanged>> ExecuteAsync(
        UpdateStorageLifecycleOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageLifecycleOptimizationChanged>.Invalid(issues);
        }

        StorageLifecycleOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageLifecycleOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageLifecycleOptimizationChanged>.Invalid(
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

        StorageLifecycleOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageLifecycleOptimizationChanged>.Success(changed);
    }
}