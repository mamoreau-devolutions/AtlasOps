namespace AtlasOps.Features.Compute.ComputeMetricMonitoring;

using AtlasOps.Features;

public sealed class ComputeMetricMonitoringService(
    IAtlasOpsCapabilityRepository<ComputeMetricMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeMetricMonitoringValidator validator = new();
    private readonly ComputeMetricMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeMetricMonitoringChanged>> ExecuteAsync(
        UpdateComputeMetricMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeMetricMonitoringChanged>.Invalid(issues);
        }

        ComputeMetricMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeMetricMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeMetricMonitoringChanged>.Invalid(
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

        ComputeMetricMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeMetricMonitoringChanged>.Success(changed);
    }
}