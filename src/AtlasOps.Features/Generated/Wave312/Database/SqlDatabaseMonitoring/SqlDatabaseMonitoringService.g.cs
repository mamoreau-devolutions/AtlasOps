namespace AtlasOps.Features.Database.SqlDatabaseMonitoring;

using AtlasOps.Features;

public sealed class SqlDatabaseMonitoringService(
    IAtlasOpsCapabilityRepository<SqlDatabaseMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SqlDatabaseMonitoringValidator validator = new();
    private readonly SqlDatabaseMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SqlDatabaseMonitoringChanged>> ExecuteAsync(
        UpdateSqlDatabaseMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SqlDatabaseMonitoringChanged>.Invalid(issues);
        }

        SqlDatabaseMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SqlDatabaseMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SqlDatabaseMonitoringChanged>.Invalid(
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

        SqlDatabaseMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SqlDatabaseMonitoringChanged>.Success(changed);
    }
}