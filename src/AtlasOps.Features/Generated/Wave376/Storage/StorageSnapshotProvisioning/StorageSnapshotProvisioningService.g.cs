namespace AtlasOps.Features.Storage.StorageSnapshotProvisioning;

using AtlasOps.Features;

public sealed class StorageSnapshotProvisioningService(
    IAtlasOpsCapabilityRepository<StorageSnapshotProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageSnapshotProvisioningValidator validator = new();
    private readonly StorageSnapshotProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageSnapshotProvisioningChanged>> ExecuteAsync(
        UpdateStorageSnapshotProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageSnapshotProvisioningChanged>.Invalid(issues);
        }

        StorageSnapshotProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageSnapshotProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageSnapshotProvisioningChanged>.Invalid(
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

        StorageSnapshotProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageSnapshotProvisioningChanged>.Success(changed);
    }
}