namespace AtlasOps.Features.Cloud.CloudDatabaseMonitoring;

using AtlasOps.Features;

public sealed class CloudDatabaseMonitoringService(
    IAtlasOpsCapabilityRepository<CloudDatabaseMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudDatabaseMonitoringValidator validator = new();
    private readonly CloudDatabaseMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudDatabaseMonitoringChanged>> ExecuteAsync(
        UpdateCloudDatabaseMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudDatabaseMonitoringChanged>.Invalid(issues);
        }

        CloudDatabaseMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudDatabaseMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudDatabaseMonitoringChanged>.Invalid(
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

        CloudDatabaseMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudDatabaseMonitoringChanged>.Success(changed);
    }
}