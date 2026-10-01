namespace AtlasOps.Features.Identity.IdentityApplicationOptimization;

using AtlasOps.Features;

public sealed class IdentityApplicationOptimizationService(
    IAtlasOpsCapabilityRepository<IdentityApplicationOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityApplicationOptimizationValidator validator = new();
    private readonly IdentityApplicationOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityApplicationOptimizationChanged>> ExecuteAsync(
        UpdateIdentityApplicationOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityApplicationOptimizationChanged>.Invalid(issues);
        }

        IdentityApplicationOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityApplicationOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityApplicationOptimizationChanged>.Invalid(
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

        IdentityApplicationOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityApplicationOptimizationChanged>.Success(changed);
    }
}