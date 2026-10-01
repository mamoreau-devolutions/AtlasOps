namespace AtlasOps.Features.Identity.IdentityRoleOptimization;

using AtlasOps.Features;

public sealed class IdentityRoleOptimizationService(
    IAtlasOpsCapabilityRepository<IdentityRoleOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityRoleOptimizationValidator validator = new();
    private readonly IdentityRoleOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityRoleOptimizationChanged>> ExecuteAsync(
        UpdateIdentityRoleOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityRoleOptimizationChanged>.Invalid(issues);
        }

        IdentityRoleOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityRoleOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityRoleOptimizationChanged>.Invalid(
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

        IdentityRoleOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityRoleOptimizationChanged>.Success(changed);
    }
}