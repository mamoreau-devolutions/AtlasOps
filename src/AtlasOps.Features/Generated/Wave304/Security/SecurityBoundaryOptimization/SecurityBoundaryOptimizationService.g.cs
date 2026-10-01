namespace AtlasOps.Features.Security.SecurityBoundaryOptimization;

using AtlasOps.Features;

public sealed class SecurityBoundaryOptimizationService(
    IAtlasOpsCapabilityRepository<SecurityBoundaryOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityBoundaryOptimizationValidator validator = new();
    private readonly SecurityBoundaryOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityBoundaryOptimizationChanged>> ExecuteAsync(
        UpdateSecurityBoundaryOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityBoundaryOptimizationChanged>.Invalid(issues);
        }

        SecurityBoundaryOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityBoundaryOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityBoundaryOptimizationChanged>.Invalid(
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

        SecurityBoundaryOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityBoundaryOptimizationChanged>.Success(changed);
    }
}