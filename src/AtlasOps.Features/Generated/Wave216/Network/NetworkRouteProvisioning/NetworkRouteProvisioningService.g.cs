namespace AtlasOps.Features.Network.NetworkRouteProvisioning;

using AtlasOps.Features;

public sealed class NetworkRouteProvisioningService(
    IAtlasOpsCapabilityRepository<NetworkRouteProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkRouteProvisioningValidator validator = new();
    private readonly NetworkRouteProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkRouteProvisioningChanged>> ExecuteAsync(
        UpdateNetworkRouteProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkRouteProvisioningChanged>.Invalid(issues);
        }

        NetworkRouteProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkRouteProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkRouteProvisioningChanged>.Invalid(
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

        NetworkRouteProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkRouteProvisioningChanged>.Success(changed);
    }
}