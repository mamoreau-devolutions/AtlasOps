namespace AtlasOps.Features.Database.DatabaseBackupRecovery;

using AtlasOps.Features;

public sealed class DatabaseBackupRecoveryService(
    IAtlasOpsCapabilityRepository<DatabaseBackupRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseBackupRecoveryValidator validator = new();
    private readonly DatabaseBackupRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseBackupRecoveryChanged>> ExecuteAsync(
        UpdateDatabaseBackupRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseBackupRecoveryChanged>.Invalid(issues);
        }

        DatabaseBackupRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseBackupRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseBackupRecoveryChanged>.Invalid(
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

        DatabaseBackupRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseBackupRecoveryChanged>.Success(changed);
    }
}