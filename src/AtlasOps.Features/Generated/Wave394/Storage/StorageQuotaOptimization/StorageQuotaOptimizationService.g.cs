namespace AtlasOps.Features.Storage.StorageQuotaOptimization;

using AtlasOps.Features;

public sealed class StorageQuotaOptimizationService(
    IAtlasOpsCapabilityRepository<StorageQuotaOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageQuotaOptimizationValidator validator = new();
    private readonly StorageQuotaOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageQuotaOptimizationChanged>> ExecuteAsync(
        UpdateStorageQuotaOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageQuotaOptimizationChanged>.Invalid(issues);
        }

        StorageQuotaOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageQuotaOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageQuotaOptimizationChanged>.Invalid(
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

        StorageQuotaOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageQuotaOptimizationChanged>.Success(changed);
    }
}