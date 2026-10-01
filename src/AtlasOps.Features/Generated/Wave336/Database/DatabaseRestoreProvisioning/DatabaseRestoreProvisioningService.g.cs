namespace AtlasOps.Features.Database.DatabaseRestoreProvisioning;

using AtlasOps.Features;

public sealed class DatabaseRestoreProvisioningService(
    IAtlasOpsCapabilityRepository<DatabaseRestoreProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseRestoreProvisioningValidator validator = new();
    private readonly DatabaseRestoreProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseRestoreProvisioningChanged>> ExecuteAsync(
        UpdateDatabaseRestoreProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseRestoreProvisioningChanged>.Invalid(issues);
        }

        DatabaseRestoreProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseRestoreProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseRestoreProvisioningChanged>.Invalid(
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

        DatabaseRestoreProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseRestoreProvisioningChanged>.Success(changed);
    }
}