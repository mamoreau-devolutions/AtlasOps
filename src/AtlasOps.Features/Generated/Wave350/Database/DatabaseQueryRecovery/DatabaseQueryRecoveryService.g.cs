namespace AtlasOps.Features.Database.DatabaseQueryRecovery;

using AtlasOps.Features;

public sealed class DatabaseQueryRecoveryService(
    IAtlasOpsCapabilityRepository<DatabaseQueryRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseQueryRecoveryValidator validator = new();
    private readonly DatabaseQueryRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseQueryRecoveryChanged>> ExecuteAsync(
        UpdateDatabaseQueryRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseQueryRecoveryChanged>.Invalid(issues);
        }

        DatabaseQueryRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseQueryRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseQueryRecoveryChanged>.Invalid(
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

        DatabaseQueryRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseQueryRecoveryChanged>.Success(changed);
    }
}