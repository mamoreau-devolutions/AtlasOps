namespace AtlasOps.Features.Database.DatabaseIndexProvisioning;

using AtlasOps.Features;

public sealed class DatabaseIndexProvisioningService(
    IAtlasOpsCapabilityRepository<DatabaseIndexProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseIndexProvisioningValidator validator = new();
    private readonly DatabaseIndexProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseIndexProvisioningChanged>> ExecuteAsync(
        UpdateDatabaseIndexProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseIndexProvisioningChanged>.Invalid(issues);
        }

        DatabaseIndexProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseIndexProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseIndexProvisioningChanged>.Invalid(
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

        DatabaseIndexProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseIndexProvisioningChanged>.Success(changed);
    }
}