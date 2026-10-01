namespace AtlasOps.Features.Compute.ComputeImageMonitoring;

using AtlasOps.Features;

public sealed class ComputeImageMonitoringService(
    IAtlasOpsCapabilityRepository<ComputeImageMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeImageMonitoringValidator validator = new();
    private readonly ComputeImageMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeImageMonitoringChanged>> ExecuteAsync(
        UpdateComputeImageMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeImageMonitoringChanged>.Invalid(issues);
        }

        ComputeImageMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeImageMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeImageMonitoringChanged>.Invalid(
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

        ComputeImageMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeImageMonitoringChanged>.Success(changed);
    }
}