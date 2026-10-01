namespace AtlasOps.Features.Identity.IdentityLifecycleOptimization;

using AtlasOps.Features;

public sealed class IdentityLifecycleOptimizationService(
    IAtlasOpsCapabilityRepository<IdentityLifecycleOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityLifecycleOptimizationValidator validator = new();
    private readonly IdentityLifecycleOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityLifecycleOptimizationChanged>> ExecuteAsync(
        UpdateIdentityLifecycleOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityLifecycleOptimizationChanged>.Invalid(issues);
        }

        IdentityLifecycleOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityLifecycleOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityLifecycleOptimizationChanged>.Invalid(
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

        IdentityLifecycleOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityLifecycleOptimizationChanged>.Success(changed);
    }
}