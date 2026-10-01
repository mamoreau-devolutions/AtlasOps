namespace AtlasOps.Features.Edge.EdgePolicyOptimization;

using AtlasOps.Features;

public sealed class EdgePolicyOptimizationService(
    IAtlasOpsCapabilityRepository<EdgePolicyOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgePolicyOptimizationValidator validator = new();
    private readonly EdgePolicyOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgePolicyOptimizationChanged>> ExecuteAsync(
        UpdateEdgePolicyOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgePolicyOptimizationChanged>.Invalid(issues);
        }

        EdgePolicyOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgePolicyOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgePolicyOptimizationChanged>.Invalid(
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

        EdgePolicyOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgePolicyOptimizationChanged>.Success(changed);
    }
}