namespace AtlasOps.Features.Security.SecurityPatchOptimization;

using AtlasOps.Features;

public sealed class SecurityPatchOptimizationService(
    IAtlasOpsCapabilityRepository<SecurityPatchOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityPatchOptimizationValidator validator = new();
    private readonly SecurityPatchOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityPatchOptimizationChanged>> ExecuteAsync(
        UpdateSecurityPatchOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityPatchOptimizationChanged>.Invalid(issues);
        }

        SecurityPatchOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityPatchOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityPatchOptimizationChanged>.Invalid(
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

        SecurityPatchOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityPatchOptimizationChanged>.Success(changed);
    }
}