namespace AtlasOps.Features.Network.NetworkLoadBalancerProvisioning;

using AtlasOps.Features;

public sealed class NetworkLoadBalancerProvisioningService(
    IAtlasOpsCapabilityRepository<NetworkLoadBalancerProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkLoadBalancerProvisioningValidator validator = new();
    private readonly NetworkLoadBalancerProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkLoadBalancerProvisioningChanged>> ExecuteAsync(
        UpdateNetworkLoadBalancerProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerProvisioningChanged>.Invalid(issues);
        }

        NetworkLoadBalancerProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkLoadBalancerProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerProvisioningChanged>.Invalid(
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

        NetworkLoadBalancerProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkLoadBalancerProvisioningChanged>.Success(changed);
    }
}