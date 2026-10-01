namespace AtlasOps.Features.Storage.StorageArchiveProvisioning;

using AtlasOps.Features;

public sealed class StorageArchiveProvisioningService(
    IAtlasOpsCapabilityRepository<StorageArchiveProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageArchiveProvisioningValidator validator = new();
    private readonly StorageArchiveProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageArchiveProvisioningChanged>> ExecuteAsync(
        UpdateStorageArchiveProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageArchiveProvisioningChanged>.Invalid(issues);
        }

        StorageArchiveProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageArchiveProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageArchiveProvisioningChanged>.Invalid(
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

        StorageArchiveProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageArchiveProvisioningChanged>.Success(changed);
    }
}