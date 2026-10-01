namespace AtlasOps.Features.Edge.EdgeNetworkOptimization;

using AtlasOps.Features;

public sealed class EdgeNetworkOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeNetworkOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeNetworkOptimizationValidator validator = new();
    private readonly EdgeNetworkOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeNetworkOptimizationChanged>> ExecuteAsync(
        UpdateEdgeNetworkOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeNetworkOptimizationChanged>.Invalid(issues);
        }

        EdgeNetworkOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeNetworkOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeNetworkOptimizationChanged>.Invalid(
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

        EdgeNetworkOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeNetworkOptimizationChanged>.Success(changed);
    }
}