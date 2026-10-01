namespace AtlasOps.Features.Database.NoSqlDatabaseRecovery;

using AtlasOps.Features;

public sealed class NoSqlDatabaseRecoveryService(
    IAtlasOpsCapabilityRepository<NoSqlDatabaseRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NoSqlDatabaseRecoveryValidator validator = new();
    private readonly NoSqlDatabaseRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NoSqlDatabaseRecoveryChanged>> ExecuteAsync(
        UpdateNoSqlDatabaseRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NoSqlDatabaseRecoveryChanged>.Invalid(issues);
        }

        NoSqlDatabaseRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NoSqlDatabaseRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NoSqlDatabaseRecoveryChanged>.Invalid(
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

        NoSqlDatabaseRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NoSqlDatabaseRecoveryChanged>.Success(changed);
    }
}