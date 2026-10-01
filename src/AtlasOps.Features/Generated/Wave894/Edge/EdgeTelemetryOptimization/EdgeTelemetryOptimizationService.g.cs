namespace AtlasOps.Features.Edge.EdgeTelemetryOptimization;

using AtlasOps.Features;

public sealed class EdgeTelemetryOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeTelemetryOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeTelemetryOptimizationValidator validator = new();
    private readonly EdgeTelemetryOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeTelemetryOptimizationChanged>> ExecuteAsync(
        UpdateEdgeTelemetryOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeTelemetryOptimizationChanged>.Invalid(issues);
        }

        EdgeTelemetryOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeTelemetryOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeTelemetryOptimizationChanged>.Invalid(
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

        EdgeTelemetryOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeTelemetryOptimizationChanged>.Success(changed);
    }
}