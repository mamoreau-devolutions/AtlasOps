namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowMonitoring;

using AtlasOps.Features;

public sealed class MaintenanceWindowMonitoringService(
    IAtlasOpsCapabilityRepository<MaintenanceWindowMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MaintenanceWindowMonitoringValidator validator = new();
    private readonly MaintenanceWindowMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MaintenanceWindowMonitoringChanged>> ExecuteAsync(
        UpdateMaintenanceWindowMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MaintenanceWindowMonitoringChanged>.Invalid(issues);
        }

        MaintenanceWindowMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MaintenanceWindowMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MaintenanceWindowMonitoringChanged>.Invalid(
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

        MaintenanceWindowMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MaintenanceWindowMonitoringChanged>.Success(changed);
    }
}