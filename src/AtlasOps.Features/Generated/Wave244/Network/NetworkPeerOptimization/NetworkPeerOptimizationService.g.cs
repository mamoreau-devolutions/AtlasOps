namespace AtlasOps.Features.Network.NetworkPeerOptimization;

using AtlasOps.Features;

public sealed class NetworkPeerOptimizationService(
    IAtlasOpsCapabilityRepository<NetworkPeerOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkPeerOptimizationValidator validator = new();
    private readonly NetworkPeerOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkPeerOptimizationChanged>> ExecuteAsync(
        UpdateNetworkPeerOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkPeerOptimizationChanged>.Invalid(issues);
        }

        NetworkPeerOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkPeerOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkPeerOptimizationChanged>.Invalid(
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

        NetworkPeerOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkPeerOptimizationChanged>.Success(changed);
    }
}