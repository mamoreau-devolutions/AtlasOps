namespace AtlasOps.Features.Network.NetworkLoadBalancerGovernance;

using AtlasOps.Features;

public sealed class NetworkLoadBalancerGovernanceService(
    IAtlasOpsCapabilityRepository<NetworkLoadBalancerGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkLoadBalancerGovernanceValidator validator = new();
    private readonly NetworkLoadBalancerGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkLoadBalancerGovernanceChanged>> ExecuteAsync(
        UpdateNetworkLoadBalancerGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerGovernanceChanged>.Invalid(issues);
        }

        NetworkLoadBalancerGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkLoadBalancerGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerGovernanceChanged>.Invalid(
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

        NetworkLoadBalancerGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkLoadBalancerGovernanceChanged>.Success(changed);
    }
}