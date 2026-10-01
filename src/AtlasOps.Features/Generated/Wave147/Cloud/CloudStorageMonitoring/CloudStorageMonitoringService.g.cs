namespace AtlasOps.Features.Cloud.CloudStorageMonitoring;

using AtlasOps.Features;

public sealed class CloudStorageMonitoringService(
    IAtlasOpsCapabilityRepository<CloudStorageMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudStorageMonitoringValidator validator = new();
    private readonly CloudStorageMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudStorageMonitoringChanged>> ExecuteAsync(
        UpdateCloudStorageMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudStorageMonitoringChanged>.Invalid(issues);
        }

        CloudStorageMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudStorageMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudStorageMonitoringChanged>.Invalid(
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

        CloudStorageMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudStorageMonitoringChanged>.Success(changed);
    }
}