namespace AtlasOps.Features.Storage.StorageEncryptionProvisioning;

using AtlasOps.Features;

public sealed class StorageEncryptionProvisioningService(
    IAtlasOpsCapabilityRepository<StorageEncryptionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageEncryptionProvisioningValidator validator = new();
    private readonly StorageEncryptionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageEncryptionProvisioningChanged>> ExecuteAsync(
        UpdateStorageEncryptionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageEncryptionProvisioningChanged>.Invalid(issues);
        }

        StorageEncryptionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageEncryptionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageEncryptionProvisioningChanged>.Invalid(
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

        StorageEncryptionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageEncryptionProvisioningChanged>.Success(changed);
    }
}