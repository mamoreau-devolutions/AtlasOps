namespace AtlasOps.Features.Database.DatabaseQueryProvisioning;

using AtlasOps.Features;

public sealed class DatabaseQueryProvisioningService(
    IAtlasOpsCapabilityRepository<DatabaseQueryProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseQueryProvisioningValidator validator = new();
    private readonly DatabaseQueryProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseQueryProvisioningChanged>> ExecuteAsync(
        UpdateDatabaseQueryProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseQueryProvisioningChanged>.Invalid(issues);
        }

        DatabaseQueryProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseQueryProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseQueryProvisioningChanged>.Invalid(
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

        DatabaseQueryProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseQueryProvisioningChanged>.Success(changed);
    }
}