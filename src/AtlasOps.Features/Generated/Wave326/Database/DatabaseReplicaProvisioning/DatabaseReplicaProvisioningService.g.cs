namespace AtlasOps.Features.Database.DatabaseReplicaProvisioning;

using AtlasOps.Features;

public sealed class DatabaseReplicaProvisioningService(
    IAtlasOpsCapabilityRepository<DatabaseReplicaProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseReplicaProvisioningValidator validator = new();
    private readonly DatabaseReplicaProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseReplicaProvisioningChanged>> ExecuteAsync(
        UpdateDatabaseReplicaProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseReplicaProvisioningChanged>.Invalid(issues);
        }

        DatabaseReplicaProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseReplicaProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseReplicaProvisioningChanged>.Invalid(
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

        DatabaseReplicaProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseReplicaProvisioningChanged>.Success(changed);
    }
}