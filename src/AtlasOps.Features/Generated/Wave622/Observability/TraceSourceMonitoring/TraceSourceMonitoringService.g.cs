namespace AtlasOps.Features.Observability.TraceSourceMonitoring;

using AtlasOps.Features;

public sealed class TraceSourceMonitoringService(
    IAtlasOpsCapabilityRepository<TraceSourceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly TraceSourceMonitoringValidator validator = new();
    private readonly TraceSourceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TraceSourceMonitoringChanged>> ExecuteAsync(
        UpdateTraceSourceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TraceSourceMonitoringChanged>.Invalid(issues);
        }

        TraceSourceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TraceSourceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TraceSourceMonitoringChanged>.Invalid(
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

        TraceSourceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TraceSourceMonitoringChanged>.Success(changed);
    }
}