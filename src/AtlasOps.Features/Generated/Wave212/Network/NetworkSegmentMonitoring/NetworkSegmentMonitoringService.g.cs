namespace AtlasOps.Features.Network.NetworkSegmentMonitoring;

using AtlasOps.Features;

public sealed class NetworkSegmentMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkSegmentMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkSegmentMonitoringValidator validator = new();
    private readonly NetworkSegmentMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkSegmentMonitoringChanged>> ExecuteAsync(
        UpdateNetworkSegmentMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkSegmentMonitoringChanged>.Invalid(issues);
        }

        NetworkSegmentMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkSegmentMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkSegmentMonitoringChanged>.Invalid(
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

        NetworkSegmentMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkSegmentMonitoringChanged>.Success(changed);
    }
}