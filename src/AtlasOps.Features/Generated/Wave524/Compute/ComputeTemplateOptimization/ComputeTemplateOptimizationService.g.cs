namespace AtlasOps.Features.Compute.ComputeTemplateOptimization;

using AtlasOps.Features;

public sealed class ComputeTemplateOptimizationService(
    IAtlasOpsCapabilityRepository<ComputeTemplateOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeTemplateOptimizationValidator validator = new();
    private readonly ComputeTemplateOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeTemplateOptimizationChanged>> ExecuteAsync(
        UpdateComputeTemplateOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeTemplateOptimizationChanged>.Invalid(issues);
        }

        ComputeTemplateOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeTemplateOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeTemplateOptimizationChanged>.Invalid(
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

        ComputeTemplateOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeTemplateOptimizationChanged>.Success(changed);
    }
}