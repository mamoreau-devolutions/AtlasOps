namespace AtlasOps.Features.Identity.IdentityProviderOptimization;

using AtlasOps.Features;

public sealed class IdentityProviderOptimizationService(
    IAtlasOpsCapabilityRepository<IdentityProviderOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityProviderOptimizationValidator validator = new();
    private readonly IdentityProviderOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityProviderOptimizationChanged>> ExecuteAsync(
        UpdateIdentityProviderOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityProviderOptimizationChanged>.Invalid(issues);
        }

        IdentityProviderOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityProviderOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityProviderOptimizationChanged>.Invalid(
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

        IdentityProviderOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityProviderOptimizationChanged>.Success(changed);
    }
}