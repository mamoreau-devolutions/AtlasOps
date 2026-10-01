namespace AtlasOps.Features.Database.SqlDatabaseProvisioning;

using AtlasOps.Features;

public sealed class SqlDatabaseProvisioningService(
    IAtlasOpsCapabilityRepository<SqlDatabaseProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SqlDatabaseProvisioningValidator validator = new();
    private readonly SqlDatabaseProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SqlDatabaseProvisioningChanged>> ExecuteAsync(
        UpdateSqlDatabaseProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SqlDatabaseProvisioningChanged>.Invalid(issues);
        }

        SqlDatabaseProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SqlDatabaseProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SqlDatabaseProvisioningChanged>.Invalid(
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

        SqlDatabaseProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SqlDatabaseProvisioningChanged>.Success(changed);
    }
}