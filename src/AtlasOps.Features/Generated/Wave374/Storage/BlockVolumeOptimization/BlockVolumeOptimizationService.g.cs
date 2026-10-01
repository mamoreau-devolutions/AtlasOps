namespace AtlasOps.Features.Storage.BlockVolumeOptimization;

using AtlasOps.Features;

public sealed class BlockVolumeOptimizationService(
    IAtlasOpsCapabilityRepository<BlockVolumeOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly BlockVolumeOptimizationValidator validator = new();
    private readonly BlockVolumeOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BlockVolumeOptimizationChanged>> ExecuteAsync(
        UpdateBlockVolumeOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BlockVolumeOptimizationChanged>.Invalid(issues);
        }

        BlockVolumeOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BlockVolumeOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BlockVolumeOptimizationChanged>.Invalid(
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

        BlockVolumeOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BlockVolumeOptimizationChanged>.Success(changed);
    }
}