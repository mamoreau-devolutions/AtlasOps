namespace AtlasOps.Features.Compute.ComputeLifecycleMonitoring;

using AtlasOps.Features;

public sealed class ComputeLifecycleMonitoringService(
    IAtlasOpsCapabilityRepository<ComputeLifecycleMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeLifecycleMonitoringValidator validator = new();
    private readonly ComputeLifecycleMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeLifecycleMonitoringChanged>> ExecuteAsync(
        UpdateComputeLifecycleMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeLifecycleMonitoringChanged>.Invalid(issues);
        }

        ComputeLifecycleMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeLifecycleMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeLifecycleMonitoringChanged>.Invalid(
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

        ComputeLifecycleMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeLifecycleMonitoringChanged>.Success(changed);
    }
}