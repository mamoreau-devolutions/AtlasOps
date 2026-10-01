namespace AtlasOps.Features.Database.DatabaseSchemaMonitoring;

using AtlasOps.Features;

public sealed class DatabaseSchemaMonitoringService(
    IAtlasOpsCapabilityRepository<DatabaseSchemaMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseSchemaMonitoringValidator validator = new();
    private readonly DatabaseSchemaMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseSchemaMonitoringChanged>> ExecuteAsync(
        UpdateDatabaseSchemaMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseSchemaMonitoringChanged>.Invalid(issues);
        }

        DatabaseSchemaMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseSchemaMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseSchemaMonitoringChanged>.Invalid(
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

        DatabaseSchemaMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseSchemaMonitoringChanged>.Success(changed);
    }
}