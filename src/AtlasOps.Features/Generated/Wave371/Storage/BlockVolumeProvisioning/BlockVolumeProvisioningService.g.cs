namespace AtlasOps.Features.Storage.BlockVolumeProvisioning;

using AtlasOps.Features;

public sealed class BlockVolumeProvisioningService(
    IAtlasOpsCapabilityRepository<BlockVolumeProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly BlockVolumeProvisioningValidator validator = new();
    private readonly BlockVolumeProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BlockVolumeProvisioningChanged>> ExecuteAsync(
        UpdateBlockVolumeProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BlockVolumeProvisioningChanged>.Invalid(issues);
        }

        BlockVolumeProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BlockVolumeProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BlockVolumeProvisioningChanged>.Invalid(
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

        BlockVolumeProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BlockVolumeProvisioningChanged>.Success(changed);
    }
}