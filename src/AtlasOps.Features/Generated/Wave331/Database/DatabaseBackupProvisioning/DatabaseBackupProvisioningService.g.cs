namespace AtlasOps.Features.Database.DatabaseBackupProvisioning;

using AtlasOps.Features;

public sealed class DatabaseBackupProvisioningService(
    IAtlasOpsCapabilityRepository<DatabaseBackupProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseBackupProvisioningValidator validator = new();
    private readonly DatabaseBackupProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseBackupProvisioningChanged>> ExecuteAsync(
        UpdateDatabaseBackupProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseBackupProvisioningChanged>.Invalid(issues);
        }

        DatabaseBackupProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseBackupProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseBackupProvisioningChanged>.Invalid(
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

        DatabaseBackupProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseBackupProvisioningChanged>.Success(changed);
    }
}