namespace AtlasOps.Features.Network.NetworkFirewallGovernance;

using AtlasOps.Features;

public sealed class NetworkFirewallGovernanceService(
    IAtlasOpsCapabilityRepository<NetworkFirewallGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkFirewallGovernanceValidator validator = new();
    private readonly NetworkFirewallGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkFirewallGovernanceChanged>> ExecuteAsync(
        UpdateNetworkFirewallGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkFirewallGovernanceChanged>.Invalid(issues);
        }

        NetworkFirewallGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkFirewallGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkFirewallGovernanceChanged>.Invalid(
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

        NetworkFirewallGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkFirewallGovernanceChanged>.Success(changed);
    }
}