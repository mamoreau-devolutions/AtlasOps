namespace AtlasOps.Features.Compute.ComputeConsoleMonitoring;

using AtlasOps.Features;

public sealed class ComputeConsoleMonitoringService(
    IAtlasOpsCapabilityRepository<ComputeConsoleMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeConsoleMonitoringValidator validator = new();
    private readonly ComputeConsoleMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeConsoleMonitoringChanged>> ExecuteAsync(
        UpdateComputeConsoleMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeConsoleMonitoringChanged>.Invalid(issues);
        }

        ComputeConsoleMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeConsoleMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeConsoleMonitoringChanged>.Invalid(
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

        ComputeConsoleMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeConsoleMonitoringChanged>.Success(changed);
    }
}