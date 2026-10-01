namespace AtlasOps.Features.Network.NetworkDnsZoneProvisioning;

using AtlasOps.Features;

public sealed class NetworkDnsZoneProvisioningService(
    IAtlasOpsCapabilityRepository<NetworkDnsZoneProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkDnsZoneProvisioningValidator validator = new();
    private readonly NetworkDnsZoneProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkDnsZoneProvisioningChanged>> ExecuteAsync(
        UpdateNetworkDnsZoneProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkDnsZoneProvisioningChanged>.Invalid(issues);
        }

        NetworkDnsZoneProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkDnsZoneProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkDnsZoneProvisioningChanged>.Invalid(
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

        NetworkDnsZoneProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkDnsZoneProvisioningChanged>.Success(changed);
    }
}