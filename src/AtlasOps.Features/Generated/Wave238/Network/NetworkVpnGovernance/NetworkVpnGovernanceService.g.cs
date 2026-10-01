namespace AtlasOps.Features.Network.NetworkVpnGovernance;

using AtlasOps.Features;

public sealed class NetworkVpnGovernanceService(
    IAtlasOpsCapabilityRepository<NetworkVpnGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkVpnGovernanceValidator validator = new();
    private readonly NetworkVpnGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkVpnGovernanceChanged>> ExecuteAsync(
        UpdateNetworkVpnGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkVpnGovernanceChanged>.Invalid(issues);
        }

        NetworkVpnGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkVpnGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkVpnGovernanceChanged>.Invalid(
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

        NetworkVpnGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkVpnGovernanceChanged>.Success(changed);
    }
}