namespace AtlasOps.Features.Database.DatabaseMaintenanceMonitoring;

using AtlasOps.Features;

public sealed class DatabaseMaintenanceMonitoringService(
    IAtlasOpsCapabilityRepository<DatabaseMaintenanceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseMaintenanceMonitoringValidator validator = new();
    private readonly DatabaseMaintenanceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseMaintenanceMonitoringChanged>> ExecuteAsync(
        UpdateDatabaseMaintenanceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseMaintenanceMonitoringChanged>.Invalid(issues);
        }

        DatabaseMaintenanceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseMaintenanceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseMaintenanceMonitoringChanged>.Invalid(
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

        DatabaseMaintenanceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseMaintenanceMonitoringChanged>.Success(changed);
    }
}