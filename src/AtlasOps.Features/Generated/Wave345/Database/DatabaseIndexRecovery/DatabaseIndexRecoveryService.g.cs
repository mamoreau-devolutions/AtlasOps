namespace AtlasOps.Features.Database.DatabaseIndexRecovery;

using AtlasOps.Features;

public sealed class DatabaseIndexRecoveryService(
    IAtlasOpsCapabilityRepository<DatabaseIndexRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseIndexRecoveryValidator validator = new();
    private readonly DatabaseIndexRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseIndexRecoveryChanged>> ExecuteAsync(
        UpdateDatabaseIndexRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseIndexRecoveryChanged>.Invalid(issues);
        }

        DatabaseIndexRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseIndexRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseIndexRecoveryChanged>.Invalid(
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

        DatabaseIndexRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseIndexRecoveryChanged>.Success(changed);
    }
}