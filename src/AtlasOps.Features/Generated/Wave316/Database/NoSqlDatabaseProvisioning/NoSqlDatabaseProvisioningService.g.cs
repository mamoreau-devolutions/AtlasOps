namespace AtlasOps.Features.Database.NoSqlDatabaseProvisioning;

using AtlasOps.Features;

public sealed class NoSqlDatabaseProvisioningService(
    IAtlasOpsCapabilityRepository<NoSqlDatabaseProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly NoSqlDatabaseProvisioningValidator validator = new();
    private readonly NoSqlDatabaseProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NoSqlDatabaseProvisioningChanged>> ExecuteAsync(
        UpdateNoSqlDatabaseProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NoSqlDatabaseProvisioningChanged>.Invalid(issues);
        }

        NoSqlDatabaseProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NoSqlDatabaseProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NoSqlDatabaseProvisioningChanged>.Invalid(
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

        NoSqlDatabaseProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NoSqlDatabaseProvisioningChanged>.Success(changed);
    }
}