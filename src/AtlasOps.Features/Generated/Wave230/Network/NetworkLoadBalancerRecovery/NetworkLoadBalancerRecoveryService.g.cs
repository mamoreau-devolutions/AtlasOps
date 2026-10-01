namespace AtlasOps.Features.Network.NetworkLoadBalancerRecovery;

using AtlasOps.Features;

public sealed class NetworkLoadBalancerRecoveryService(
    IAtlasOpsCapabilityRepository<NetworkLoadBalancerRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkLoadBalancerRecoveryValidator validator = new();
    private readonly NetworkLoadBalancerRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkLoadBalancerRecoveryChanged>> ExecuteAsync(
        UpdateNetworkLoadBalancerRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerRecoveryChanged>.Invalid(issues);
        }

        NetworkLoadBalancerRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkLoadBalancerRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkLoadBalancerRecoveryChanged>.Invalid(
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

        NetworkLoadBalancerRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkLoadBalancerRecoveryChanged>.Success(changed);
    }
}