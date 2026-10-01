namespace AtlasOps.Features.Security.SecurityIdentityOptimization;

using AtlasOps.Features;

public sealed class SecurityIdentityOptimizationService(
    IAtlasOpsCapabilityRepository<SecurityIdentityOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityIdentityOptimizationValidator validator = new();
    private readonly SecurityIdentityOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityIdentityOptimizationChanged>> ExecuteAsync(
        UpdateSecurityIdentityOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityIdentityOptimizationChanged>.Invalid(issues);
        }

        SecurityIdentityOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityIdentityOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityIdentityOptimizationChanged>.Invalid(
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

        SecurityIdentityOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityIdentityOptimizationChanged>.Success(changed);
    }
}