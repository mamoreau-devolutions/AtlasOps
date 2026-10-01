namespace AtlasOps.Features.Observability.TraceSpanOptimization;

using AtlasOps.Features;

public sealed class TraceSpanOptimizationService(
    IAtlasOpsCapabilityRepository<TraceSpanOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly TraceSpanOptimizationValidator validator = new();
    private readonly TraceSpanOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TraceSpanOptimizationChanged>> ExecuteAsync(
        UpdateTraceSpanOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TraceSpanOptimizationChanged>.Invalid(issues);
        }

        TraceSpanOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TraceSpanOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TraceSpanOptimizationChanged>.Invalid(
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

        TraceSpanOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TraceSpanOptimizationChanged>.Success(changed);
    }
}