namespace AtlasOps.Features.Network.NetworkPolicyProvisioning;

using AtlasOps.Features;

public sealed class NetworkPolicyProvisioningService(
    IAtlasOpsCapabilityRepository<NetworkPolicyProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkPolicyProvisioningValidator validator = new();
    private readonly NetworkPolicyProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkPolicyProvisioningChanged>> ExecuteAsync(
        UpdateNetworkPolicyProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkPolicyProvisioningChanged>.Invalid(issues);
        }

        NetworkPolicyProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkPolicyProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkPolicyProvisioningChanged>.Invalid(
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

        NetworkPolicyProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkPolicyProvisioningChanged>.Success(changed);
    }
}