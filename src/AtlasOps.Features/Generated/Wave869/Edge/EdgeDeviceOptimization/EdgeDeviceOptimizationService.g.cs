namespace AtlasOps.Features.Edge.EdgeDeviceOptimization;

using AtlasOps.Features;

public sealed class EdgeDeviceOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeDeviceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeviceOptimizationValidator validator = new();
    private readonly EdgeDeviceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeviceOptimizationChanged>> ExecuteAsync(
        UpdateEdgeDeviceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeviceOptimizationChanged>.Invalid(issues);
        }

        EdgeDeviceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeviceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeviceOptimizationChanged>.Invalid(
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

        EdgeDeviceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeviceOptimizationChanged>.Success(changed);
    }
}