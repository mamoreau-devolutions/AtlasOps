namespace AtlasOps.Features.Storage.StorageQuotaProvisioning;

using AtlasOps.Features;

public sealed class StorageQuotaProvisioningService(
    IAtlasOpsCapabilityRepository<StorageQuotaProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageQuotaProvisioningValidator validator = new();
    private readonly StorageQuotaProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageQuotaProvisioningChanged>> ExecuteAsync(
        UpdateStorageQuotaProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageQuotaProvisioningChanged>.Invalid(issues);
        }

        StorageQuotaProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageQuotaProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageQuotaProvisioningChanged>.Invalid(
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

        StorageQuotaProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageQuotaProvisioningChanged>.Success(changed);
    }
}