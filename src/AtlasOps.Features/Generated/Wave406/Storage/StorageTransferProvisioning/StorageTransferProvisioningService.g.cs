namespace AtlasOps.Features.Storage.StorageTransferProvisioning;

using AtlasOps.Features;

public sealed class StorageTransferProvisioningService(
    IAtlasOpsCapabilityRepository<StorageTransferProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageTransferProvisioningValidator validator = new();
    private readonly StorageTransferProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageTransferProvisioningChanged>> ExecuteAsync(
        UpdateStorageTransferProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageTransferProvisioningChanged>.Invalid(issues);
        }

        StorageTransferProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageTransferProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageTransferProvisioningChanged>.Invalid(
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

        StorageTransferProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageTransferProvisioningChanged>.Success(changed);
    }
}