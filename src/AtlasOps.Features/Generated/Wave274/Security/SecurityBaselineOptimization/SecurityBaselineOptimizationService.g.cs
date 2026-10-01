namespace AtlasOps.Features.Security.SecurityBaselineOptimization;

using AtlasOps.Features;

public sealed class SecurityBaselineOptimizationService(
    IAtlasOpsCapabilityRepository<SecurityBaselineOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityBaselineOptimizationValidator validator = new();
    private readonly SecurityBaselineOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityBaselineOptimizationChanged>> ExecuteAsync(
        UpdateSecurityBaselineOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityBaselineOptimizationChanged>.Invalid(issues);
        }

        SecurityBaselineOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityBaselineOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityBaselineOptimizationChanged>.Invalid(
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

        SecurityBaselineOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityBaselineOptimizationChanged>.Success(changed);
    }
}