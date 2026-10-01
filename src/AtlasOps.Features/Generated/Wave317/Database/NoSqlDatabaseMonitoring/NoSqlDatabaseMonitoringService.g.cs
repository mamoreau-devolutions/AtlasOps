namespace AtlasOps.Features.Database.NoSqlDatabaseMonitoring;

using AtlasOps.Features;

public sealed class NoSqlDatabaseMonitoringService(
    IAtlasOpsCapabilityRepository<NoSqlDatabaseMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NoSqlDatabaseMonitoringValidator validator = new();
    private readonly NoSqlDatabaseMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NoSqlDatabaseMonitoringChanged>> ExecuteAsync(
        UpdateNoSqlDatabaseMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NoSqlDatabaseMonitoringChanged>.Invalid(issues);
        }

        NoSqlDatabaseMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NoSqlDatabaseMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NoSqlDatabaseMonitoringChanged>.Invalid(
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

        NoSqlDatabaseMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NoSqlDatabaseMonitoringChanged>.Success(changed);
    }
}