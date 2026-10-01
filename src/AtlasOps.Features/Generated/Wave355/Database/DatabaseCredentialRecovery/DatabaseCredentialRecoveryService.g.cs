namespace AtlasOps.Features.Database.DatabaseCredentialRecovery;

using AtlasOps.Features;

public sealed class DatabaseCredentialRecoveryService(
    IAtlasOpsCapabilityRepository<DatabaseCredentialRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseCredentialRecoveryValidator validator = new();
    private readonly DatabaseCredentialRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseCredentialRecoveryChanged>> ExecuteAsync(
        UpdateDatabaseCredentialRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseCredentialRecoveryChanged>.Invalid(issues);
        }

        DatabaseCredentialRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseCredentialRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseCredentialRecoveryChanged>.Invalid(
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

        DatabaseCredentialRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseCredentialRecoveryChanged>.Success(changed);
    }
}