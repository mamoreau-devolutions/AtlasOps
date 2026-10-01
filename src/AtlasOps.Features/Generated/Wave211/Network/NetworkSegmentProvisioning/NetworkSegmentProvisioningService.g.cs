namespace AtlasOps.Features.Network.NetworkSegmentProvisioning;

using AtlasOps.Features;

public sealed class NetworkSegmentProvisioningService(
    IAtlasOpsCapabilityRepository<NetworkSegmentProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkSegmentProvisioningValidator validator = new();
    private readonly NetworkSegmentProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkSegmentProvisioningChanged>> ExecuteAsync(
        UpdateNetworkSegmentProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkSegmentProvisioningChanged>.Invalid(issues);
        }

        NetworkSegmentProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkSegmentProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkSegmentProvisioningChanged>.Invalid(
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

        NetworkSegmentProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkSegmentProvisioningChanged>.Success(changed);
    }
}