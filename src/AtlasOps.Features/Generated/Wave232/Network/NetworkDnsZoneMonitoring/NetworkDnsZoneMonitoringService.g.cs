namespace AtlasOps.Features.Network.NetworkDnsZoneMonitoring;

using AtlasOps.Features;

public sealed class NetworkDnsZoneMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkDnsZoneMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkDnsZoneMonitoringValidator validator = new();
    private readonly NetworkDnsZoneMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkDnsZoneMonitoringChanged>> ExecuteAsync(
        UpdateNetworkDnsZoneMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkDnsZoneMonitoringChanged>.Invalid(issues);
        }

        NetworkDnsZoneMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkDnsZoneMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkDnsZoneMonitoringChanged>.Invalid(
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

        NetworkDnsZoneMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkDnsZoneMonitoringChanged>.Success(changed);
    }
}