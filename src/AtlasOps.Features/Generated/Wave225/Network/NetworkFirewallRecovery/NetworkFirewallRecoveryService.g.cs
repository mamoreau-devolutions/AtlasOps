namespace AtlasOps.Features.Network.NetworkFirewallRecovery;

using AtlasOps.Features;

public sealed class NetworkFirewallRecoveryService(
    IAtlasOpsCapabilityRepository<NetworkFirewallRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkFirewallRecoveryValidator validator = new();
    private readonly NetworkFirewallRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkFirewallRecoveryChanged>> ExecuteAsync(
        UpdateNetworkFirewallRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkFirewallRecoveryChanged>.Invalid(issues);
        }

        NetworkFirewallRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkFirewallRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkFirewallRecoveryChanged>.Invalid(
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

        NetworkFirewallRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkFirewallRecoveryChanged>.Success(changed);
    }
}