namespace AtlasOps.Features.Network.NetworkAddressMonitoring;

using AtlasOps.Features;

public sealed class NetworkAddressMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkAddressMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkAddressMonitoringValidator validator = new();
    private readonly NetworkAddressMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkAddressMonitoringChanged>> ExecuteAsync(
        UpdateNetworkAddressMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkAddressMonitoringChanged>.Invalid(issues);
        }

        NetworkAddressMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkAddressMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkAddressMonitoringChanged>.Invalid(
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

        NetworkAddressMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkAddressMonitoringChanged>.Success(changed);
    }
}