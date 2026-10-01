namespace AtlasOps.Features.Network.NetworkVpnOptimization;

using AtlasOps.Features;

public sealed class NetworkVpnOptimizationService(
    IAtlasOpsCapabilityRepository<NetworkVpnOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkVpnOptimizationValidator validator = new();
    private readonly NetworkVpnOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkVpnOptimizationChanged>> ExecuteAsync(
        UpdateNetworkVpnOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkVpnOptimizationChanged>.Invalid(issues);
        }

        NetworkVpnOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkVpnOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkVpnOptimizationChanged>.Invalid(
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

        NetworkVpnOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkVpnOptimizationChanged>.Success(changed);
    }
}