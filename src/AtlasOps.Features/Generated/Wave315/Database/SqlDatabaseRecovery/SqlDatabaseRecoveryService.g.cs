namespace AtlasOps.Features.Database.SqlDatabaseRecovery;

using AtlasOps.Features;

public sealed class SqlDatabaseRecoveryService(
    IAtlasOpsCapabilityRepository<SqlDatabaseRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SqlDatabaseRecoveryValidator validator = new();
    private readonly SqlDatabaseRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SqlDatabaseRecoveryChanged>> ExecuteAsync(
        UpdateSqlDatabaseRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SqlDatabaseRecoveryChanged>.Invalid(issues);
        }

        SqlDatabaseRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SqlDatabaseRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SqlDatabaseRecoveryChanged>.Invalid(
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

        SqlDatabaseRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SqlDatabaseRecoveryChanged>.Success(changed);
    }
}