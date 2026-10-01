namespace AtlasOps.Features.Database.NoSqlDatabaseGovernance;

using AtlasOps.Features;

public sealed class NoSqlDatabaseGovernanceService(
    IAtlasOpsCapabilityRepository<NoSqlDatabaseGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NoSqlDatabaseGovernanceValidator validator = new();
    private readonly NoSqlDatabaseGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NoSqlDatabaseGovernanceChanged>> ExecuteAsync(
        UpdateNoSqlDatabaseGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NoSqlDatabaseGovernanceChanged>.Invalid(issues);
        }

        NoSqlDatabaseGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NoSqlDatabaseGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NoSqlDatabaseGovernanceChanged>.Invalid(
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

        NoSqlDatabaseGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NoSqlDatabaseGovernanceChanged>.Success(changed);
    }
}