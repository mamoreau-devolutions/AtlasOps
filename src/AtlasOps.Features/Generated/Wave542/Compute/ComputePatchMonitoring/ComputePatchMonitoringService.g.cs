namespace AtlasOps.Features.Compute.ComputePatchMonitoring;

using AtlasOps.Features;

public sealed class ComputePatchMonitoringService(
    IAtlasOpsCapabilityRepository<ComputePatchMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputePatchMonitoringValidator validator = new();
    private readonly ComputePatchMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputePatchMonitoringChanged>> ExecuteAsync(
        UpdateComputePatchMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputePatchMonitoringChanged>.Invalid(issues);
        }

        ComputePatchMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputePatchMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputePatchMonitoringChanged>.Invalid(
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

        ComputePatchMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputePatchMonitoringChanged>.Success(changed);
    }
}