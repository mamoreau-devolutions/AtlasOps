namespace AtlasOps.Features.Delivery.ReleasePipelineOptimization;

using AtlasOps.Features;

public sealed class ReleasePipelineOptimizationService(
    IAtlasOpsCapabilityRepository<ReleasePipelineOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleasePipelineOptimizationValidator validator = new();
    private readonly ReleasePipelineOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleasePipelineOptimizationChanged>> ExecuteAsync(
        UpdateReleasePipelineOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleasePipelineOptimizationChanged>.Invalid(issues);
        }

        ReleasePipelineOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleasePipelineOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleasePipelineOptimizationChanged>.Invalid(
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

        ReleasePipelineOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleasePipelineOptimizationChanged>.Success(changed);
    }
}