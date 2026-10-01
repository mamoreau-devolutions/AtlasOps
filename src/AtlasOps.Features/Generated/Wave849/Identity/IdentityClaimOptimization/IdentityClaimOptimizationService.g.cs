namespace AtlasOps.Features.Identity.IdentityClaimOptimization;

using AtlasOps.Features;

public sealed class IdentityClaimOptimizationService(
    IAtlasOpsCapabilityRepository<IdentityClaimOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityClaimOptimizationValidator validator = new();
    private readonly IdentityClaimOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityClaimOptimizationChanged>> ExecuteAsync(
        UpdateIdentityClaimOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityClaimOptimizationChanged>.Invalid(issues);
        }

        IdentityClaimOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityClaimOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityClaimOptimizationChanged>.Invalid(
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

        IdentityClaimOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityClaimOptimizationChanged>.Success(changed);
    }
}