namespace AtlasOps.Features.Database.DatabaseRestoreRecovery;

using AtlasOps.Features;

public sealed class DatabaseRestoreRecoveryService(
    IAtlasOpsCapabilityRepository<DatabaseRestoreRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseRestoreRecoveryValidator validator = new();
    private readonly DatabaseRestoreRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseRestoreRecoveryChanged>> ExecuteAsync(
        UpdateDatabaseRestoreRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseRestoreRecoveryChanged>.Invalid(issues);
        }

        DatabaseRestoreRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseRestoreRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseRestoreRecoveryChanged>.Invalid(
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

        DatabaseRestoreRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseRestoreRecoveryChanged>.Success(changed);
    }
}