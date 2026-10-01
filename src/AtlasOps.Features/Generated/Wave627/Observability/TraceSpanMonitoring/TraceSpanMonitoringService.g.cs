namespace AtlasOps.Features.Observability.TraceSpanMonitoring;

using AtlasOps.Features;

public sealed class TraceSpanMonitoringService(
    IAtlasOpsCapabilityRepository<TraceSpanMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly TraceSpanMonitoringValidator validator = new();
    private readonly TraceSpanMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TraceSpanMonitoringChanged>> ExecuteAsync(
        UpdateTraceSpanMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TraceSpanMonitoringChanged>.Invalid(issues);
        }

        TraceSpanMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TraceSpanMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TraceSpanMonitoringChanged>.Invalid(
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

        TraceSpanMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TraceSpanMonitoringChanged>.Success(changed);
    }
}