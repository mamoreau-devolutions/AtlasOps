namespace AtlasOps.Features.Network.NetworkFirewallOptimization;

using AtlasOps.Features;

public sealed class NetworkFirewallOptimizationService(
    IAtlasOpsCapabilityRepository<NetworkFirewallOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkFirewallOptimizationValidator validator = new();
    private readonly NetworkFirewallOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkFirewallOptimizationChanged>> ExecuteAsync(
        UpdateNetworkFirewallOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkFirewallOptimizationChanged>.Invalid(issues);
        }

        NetworkFirewallOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkFirewallOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkFirewallOptimizationChanged>.Invalid(
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

        NetworkFirewallOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkFirewallOptimizationChanged>.Success(changed);
    }
}