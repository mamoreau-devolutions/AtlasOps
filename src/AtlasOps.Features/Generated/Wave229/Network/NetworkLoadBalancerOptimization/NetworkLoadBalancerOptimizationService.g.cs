namespace AtlasOps.Features.Network.NetworkLoadBalancerOptimization;

using AtlasOps.Features;

public sealed class NetworkLoadBalancerOptimizationService(
    IAtlasOpsCapabilityRepository<NetworkLoadBalancerOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkLoadBalancerOptimizationValidator validator = new();
    private readonly NetworkLoadBalancerOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkLoadBalancerOptimizationChanged>> ExecuteAsync(
        UpdateNetworkLoadBalancerOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerOptimizationChanged>.Invalid(issues);
        }

        NetworkLoadBalancerOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkLoadBalancerOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerOptimizationChanged>.Invalid(
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

        NetworkLoadBalancerOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkLoadBalancerOptimizationChanged>.Success(changed);
    }
}