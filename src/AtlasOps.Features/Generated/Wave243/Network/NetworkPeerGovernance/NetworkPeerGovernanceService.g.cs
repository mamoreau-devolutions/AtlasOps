namespace AtlasOps.Features.Network.NetworkPeerGovernance;

using AtlasOps.Features;

public sealed class NetworkPeerGovernanceService(
    IAtlasOpsCapabilityRepository<NetworkPeerGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkPeerGovernanceValidator validator = new();
    private readonly NetworkPeerGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkPeerGovernanceChanged>> ExecuteAsync(
        UpdateNetworkPeerGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkPeerGovernanceChanged>.Invalid(issues);
        }

        NetworkPeerGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkPeerGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkPeerGovernanceChanged>.Invalid(
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

        NetworkPeerGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkPeerGovernanceChanged>.Success(changed);
    }
}