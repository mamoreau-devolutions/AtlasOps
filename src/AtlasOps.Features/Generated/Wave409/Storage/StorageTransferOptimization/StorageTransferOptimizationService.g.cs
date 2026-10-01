namespace AtlasOps.Features.Storage.StorageTransferOptimization;

using AtlasOps.Features;

public sealed class StorageTransferOptimizationService(
    IAtlasOpsCapabilityRepository<StorageTransferOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageTransferOptimizationValidator validator = new();
    private readonly StorageTransferOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageTransferOptimizationChanged>> ExecuteAsync(
        UpdateStorageTransferOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageTransferOptimizationChanged>.Invalid(issues);
        }

        StorageTransferOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageTransferOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageTransferOptimizationChanged>.Invalid(
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

        StorageTransferOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageTransferOptimizationChanged>.Success(changed);
    }
}