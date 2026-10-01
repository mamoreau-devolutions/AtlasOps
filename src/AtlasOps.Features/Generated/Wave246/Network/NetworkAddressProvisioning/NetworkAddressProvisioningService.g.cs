namespace AtlasOps.Features.Network.NetworkAddressProvisioning;

using AtlasOps.Features;

public sealed class NetworkAddressProvisioningService(
    IAtlasOpsCapabilityRepository<NetworkAddressProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkAddressProvisioningValidator validator = new();
    private readonly NetworkAddressProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkAddressProvisioningChanged>> ExecuteAsync(
        UpdateNetworkAddressProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkAddressProvisioningChanged>.Invalid(issues);
        }

        NetworkAddressProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkAddressProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkAddressProvisioningChanged>.Invalid(
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

        NetworkAddressProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkAddressProvisioningChanged>.Success(changed);
    }
}