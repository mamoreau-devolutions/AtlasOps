namespace AtlasOps.Features.Storage.StorageReplicationProvisioning;

using AtlasOps.Features;

public sealed class StorageReplicationProvisioningService(
    IAtlasOpsCapabilityRepository<StorageReplicationProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageReplicationProvisioningValidator validator = new();
    private readonly StorageReplicationProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageReplicationProvisioningChanged>> ExecuteAsync(
        UpdateStorageReplicationProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageReplicationProvisioningChanged>.Invalid(issues);
        }

        StorageReplicationProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageReplicationProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageReplicationProvisioningChanged>.Invalid(
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

        StorageReplicationProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageReplicationProvisioningChanged>.Success(changed);
    }
}