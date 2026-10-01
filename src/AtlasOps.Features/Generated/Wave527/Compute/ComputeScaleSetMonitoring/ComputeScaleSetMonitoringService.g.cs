namespace AtlasOps.Features.Compute.ComputeScaleSetMonitoring;

using AtlasOps.Features;

public sealed class ComputeScaleSetMonitoringService(
    IAtlasOpsCapabilityRepository<ComputeScaleSetMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeScaleSetMonitoringValidator validator = new();
    private readonly ComputeScaleSetMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeScaleSetMonitoringChanged>> ExecuteAsync(
        UpdateComputeScaleSetMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeScaleSetMonitoringChanged>.Invalid(issues);
        }

        ComputeScaleSetMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeScaleSetMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeScaleSetMonitoringChanged>.Invalid(
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

        ComputeScaleSetMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeScaleSetMonitoringChanged>.Success(changed);
    }
}