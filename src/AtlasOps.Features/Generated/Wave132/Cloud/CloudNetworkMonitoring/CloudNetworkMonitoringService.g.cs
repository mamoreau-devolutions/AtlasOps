namespace AtlasOps.Features.Cloud.CloudNetworkMonitoring;

using AtlasOps.Features;

public sealed class CloudNetworkMonitoringService(
    IAtlasOpsCapabilityRepository<CloudNetworkMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudNetworkMonitoringValidator validator = new();
    private readonly CloudNetworkMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudNetworkMonitoringChanged>> ExecuteAsync(
        UpdateCloudNetworkMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudNetworkMonitoringChanged>.Invalid(issues);
        }

        CloudNetworkMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudNetworkMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudNetworkMonitoringChanged>.Invalid(
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

        CloudNetworkMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudNetworkMonitoringChanged>.Success(changed);
    }
}