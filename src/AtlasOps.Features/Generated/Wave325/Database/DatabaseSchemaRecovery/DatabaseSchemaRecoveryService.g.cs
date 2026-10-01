namespace AtlasOps.Features.Database.DatabaseSchemaRecovery;

using AtlasOps.Features;

public sealed class DatabaseSchemaRecoveryService(
    IAtlasOpsCapabilityRepository<DatabaseSchemaRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseSchemaRecoveryValidator validator = new();
    private readonly DatabaseSchemaRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseSchemaRecoveryChanged>> ExecuteAsync(
        UpdateDatabaseSchemaRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseSchemaRecoveryChanged>.Invalid(issues);
        }

        DatabaseSchemaRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseSchemaRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseSchemaRecoveryChanged>.Invalid(
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

        DatabaseSchemaRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseSchemaRecoveryChanged>.Success(changed);
    }
}