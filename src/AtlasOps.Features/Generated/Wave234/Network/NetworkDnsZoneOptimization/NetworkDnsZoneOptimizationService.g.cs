namespace AtlasOps.Features.Network.NetworkDnsZoneOptimization;

using AtlasOps.Features;

public sealed class NetworkDnsZoneOptimizationService(
    IAtlasOpsCapabilityRepository<NetworkDnsZoneOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkDnsZoneOptimizationValidator validator = new();
    private readonly NetworkDnsZoneOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkDnsZoneOptimizationChanged>> ExecuteAsync(
        UpdateNetworkDnsZoneOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkDnsZoneOptimizationChanged>.Invalid(issues);
        }

        NetworkDnsZoneOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkDnsZoneOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkDnsZoneOptimizationChanged>.Invalid(
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

        NetworkDnsZoneOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkDnsZoneOptimizationChanged>.Success(changed);
    }
}