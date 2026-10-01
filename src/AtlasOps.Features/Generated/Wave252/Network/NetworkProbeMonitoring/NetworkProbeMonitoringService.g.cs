namespace AtlasOps.Features.Network.NetworkProbeMonitoring;

using AtlasOps.Features;

public sealed class NetworkProbeMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkProbeMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkProbeMonitoringValidator validator = new();
    private readonly NetworkProbeMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkProbeMonitoringChanged>> ExecuteAsync(
        UpdateNetworkProbeMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkProbeMonitoringChanged>.Invalid(issues);
        }

        NetworkProbeMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkProbeMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkProbeMonitoringChanged>.Invalid(
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

        NetworkProbeMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkProbeMonitoringChanged>.Success(changed);
    }
}