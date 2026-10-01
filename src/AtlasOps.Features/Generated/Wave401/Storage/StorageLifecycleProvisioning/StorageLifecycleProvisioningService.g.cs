namespace AtlasOps.Features.Storage.StorageLifecycleProvisioning;

using AtlasOps.Features;

public sealed class StorageLifecycleProvisioningService(
    IAtlasOpsCapabilityRepository<StorageLifecycleProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageLifecycleProvisioningValidator validator = new();
    private readonly StorageLifecycleProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageLifecycleProvisioningChanged>> ExecuteAsync(
        UpdateStorageLifecycleProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageLifecycleProvisioningChanged>.Invalid(issues);
        }

        StorageLifecycleProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageLifecycleProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageLifecycleProvisioningChanged>.Invalid(
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

        StorageLifecycleProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageLifecycleProvisioningChanged>.Success(changed);
    }
}