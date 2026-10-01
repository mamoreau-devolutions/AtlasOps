namespace AtlasOps.Features.Identity.IdentityUserOptimization;

using AtlasOps.Features;

public sealed class IdentityUserOptimizationService(
    IAtlasOpsCapabilityRepository<IdentityUserOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityUserOptimizationValidator validator = new();
    private readonly IdentityUserOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityUserOptimizationChanged>> ExecuteAsync(
        UpdateIdentityUserOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityUserOptimizationChanged>.Invalid(issues);
        }

        IdentityUserOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityUserOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityUserOptimizationChanged>.Invalid(
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

        IdentityUserOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityUserOptimizationChanged>.Success(changed);
    }
}