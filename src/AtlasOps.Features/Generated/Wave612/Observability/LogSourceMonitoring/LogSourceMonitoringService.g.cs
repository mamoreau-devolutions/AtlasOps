namespace AtlasOps.Features.Observability.LogSourceMonitoring;

using AtlasOps.Features;

public sealed class LogSourceMonitoringService(
    IAtlasOpsCapabilityRepository<LogSourceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogSourceMonitoringValidator validator = new();
    private readonly LogSourceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogSourceMonitoringChanged>> ExecuteAsync(
        UpdateLogSourceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogSourceMonitoringChanged>.Invalid(issues);
        }

        LogSourceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogSourceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogSourceMonitoringChanged>.Invalid(
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

        LogSourceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogSourceMonitoringChanged>.Success(changed);
    }
}