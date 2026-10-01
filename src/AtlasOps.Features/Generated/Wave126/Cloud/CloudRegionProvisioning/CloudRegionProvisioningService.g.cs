namespace AtlasOps.Features.Cloud.CloudRegionProvisioning;

using AtlasOps.Features;

public sealed class CloudRegionProvisioningService(
    IAtlasOpsCapabilityRepository<CloudRegionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudRegionProvisioningValidator validator = new();
    private readonly CloudRegionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudRegionProvisioningChanged>> ExecuteAsync(
        UpdateCloudRegionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudRegionProvisioningChanged>.Invalid(issues);
        }

        CloudRegionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudRegionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudRegionProvisioningChanged>.Invalid(
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

        CloudRegionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudRegionProvisioningChanged>.Success(changed);
    }
}