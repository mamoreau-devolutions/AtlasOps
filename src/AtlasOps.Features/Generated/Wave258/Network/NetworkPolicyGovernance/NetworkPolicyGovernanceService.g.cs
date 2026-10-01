namespace AtlasOps.Features.Network.NetworkPolicyGovernance;

using AtlasOps.Features;

public sealed class NetworkPolicyGovernanceService(
    IAtlasOpsCapabilityRepository<NetworkPolicyGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkPolicyGovernanceValidator validator = new();
    private readonly NetworkPolicyGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkPolicyGovernanceChanged>> ExecuteAsync(
        UpdateNetworkPolicyGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkPolicyGovernanceChanged>.Invalid(issues);
        }

        NetworkPolicyGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkPolicyGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkPolicyGovernanceChanged>.Invalid(
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

        NetworkPolicyGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkPolicyGovernanceChanged>.Success(changed);
    }
}