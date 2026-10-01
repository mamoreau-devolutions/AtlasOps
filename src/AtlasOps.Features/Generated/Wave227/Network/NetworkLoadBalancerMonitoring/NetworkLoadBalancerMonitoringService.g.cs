namespace AtlasOps.Features.Network.NetworkLoadBalancerMonitoring;

using AtlasOps.Features;

public sealed class NetworkLoadBalancerMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkLoadBalancerMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkLoadBalancerMonitoringValidator validator = new();
    private readonly NetworkLoadBalancerMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkLoadBalancerMonitoringChanged>> ExecuteAsync(
        UpdateNetworkLoadBalancerMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerMonitoringChanged>.Invalid(issues);
        }

        NetworkLoadBalancerMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkLoadBalancerMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerMonitoringChanged>.Invalid(
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

        NetworkLoadBalancerMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkLoadBalancerMonitoringChanged>.Success(changed);
    }
}