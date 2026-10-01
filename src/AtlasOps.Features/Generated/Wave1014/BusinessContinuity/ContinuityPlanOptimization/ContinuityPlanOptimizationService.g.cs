namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanOptimization;

using AtlasOps.Features;

public sealed class ContinuityPlanOptimizationService(
    IAtlasOpsCapabilityRepository<ContinuityPlanOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ContinuityPlanOptimizationValidator validator = new();
    private readonly ContinuityPlanOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ContinuityPlanOptimizationChanged>> ExecuteAsync(
        UpdateContinuityPlanOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ContinuityPlanOptimizationChanged>.Invalid(issues);
        }

        ContinuityPlanOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ContinuityPlanOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ContinuityPlanOptimizationChanged>.Invalid(
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

        ContinuityPlanOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ContinuityPlanOptimizationChanged>.Success(changed);
    }
}