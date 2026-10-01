namespace AtlasOps.Features.Edge.EdgeGatewayOptimization;

using AtlasOps.Features;

public sealed class EdgeGatewayOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeGatewayOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeGatewayOptimizationValidator validator = new();
    private readonly EdgeGatewayOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeGatewayOptimizationChanged>> ExecuteAsync(
        UpdateEdgeGatewayOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeGatewayOptimizationChanged>.Invalid(issues);
        }

        EdgeGatewayOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeGatewayOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeGatewayOptimizationChanged>.Invalid(
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

        EdgeGatewayOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeGatewayOptimizationChanged>.Success(changed);
    }
}