namespace AtlasOps.Features.Cloud.CloudNetworkProvisioning;

using AtlasOps.Features;

public sealed class CloudNetworkProvisioningService(
    IAtlasOpsCapabilityRepository<CloudNetworkProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudNetworkProvisioningValidator validator = new();
    private readonly CloudNetworkProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudNetworkProvisioningChanged>> ExecuteAsync(
        UpdateCloudNetworkProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudNetworkProvisioningChanged>.Invalid(issues);
        }

        CloudNetworkProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudNetworkProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudNetworkProvisioningChanged>.Invalid(
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

        CloudNetworkProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudNetworkProvisioningChanged>.Success(changed);
    }
}