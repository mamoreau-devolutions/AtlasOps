namespace AtlasOps.Features.Database.DatabaseSchemaProvisioning;

using AtlasOps.Features;

public sealed class DatabaseSchemaProvisioningService(
    IAtlasOpsCapabilityRepository<DatabaseSchemaProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseSchemaProvisioningValidator validator = new();
    private readonly DatabaseSchemaProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseSchemaProvisioningChanged>> ExecuteAsync(
        UpdateDatabaseSchemaProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseSchemaProvisioningChanged>.Invalid(issues);
        }

        DatabaseSchemaProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseSchemaProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseSchemaProvisioningChanged>.Invalid(
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

        DatabaseSchemaProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseSchemaProvisioningChanged>.Success(changed);
    }
}