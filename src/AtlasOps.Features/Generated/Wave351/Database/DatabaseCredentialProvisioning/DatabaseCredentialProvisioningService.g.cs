namespace AtlasOps.Features.Database.DatabaseCredentialProvisioning;

using AtlasOps.Features;

public sealed class DatabaseCredentialProvisioningService(
    IAtlasOpsCapabilityRepository<DatabaseCredentialProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseCredentialProvisioningValidator validator = new();
    private readonly DatabaseCredentialProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseCredentialProvisioningChanged>> ExecuteAsync(
        UpdateDatabaseCredentialProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseCredentialProvisioningChanged>.Invalid(issues);
        }

        DatabaseCredentialProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseCredentialProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseCredentialProvisioningChanged>.Invalid(
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

        DatabaseCredentialProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseCredentialProvisioningChanged>.Success(changed);
    }
}