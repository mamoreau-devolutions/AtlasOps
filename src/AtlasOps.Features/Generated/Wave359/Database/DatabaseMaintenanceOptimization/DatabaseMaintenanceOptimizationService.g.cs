namespace AtlasOps.Features.Database.DatabaseMaintenanceOptimization;

using AtlasOps.Features;

public sealed class DatabaseMaintenanceOptimizationService(
    IAtlasOpsCapabilityRepository<DatabaseMaintenanceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseMaintenanceOptimizationValidator validator = new();
    private readonly DatabaseMaintenanceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseMaintenanceOptimizationChanged>> ExecuteAsync(
        UpdateDatabaseMaintenanceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseMaintenanceOptimizationChanged>.Invalid(issues);
        }

        DatabaseMaintenanceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseMaintenanceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseMaintenanceOptimizationChanged>.Invalid(
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

        DatabaseMaintenanceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseMaintenanceOptimizationChanged>.Success(changed);
    }
}