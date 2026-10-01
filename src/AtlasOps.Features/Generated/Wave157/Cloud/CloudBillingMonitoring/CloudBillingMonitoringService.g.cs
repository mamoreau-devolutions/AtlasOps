namespace AtlasOps.Features.Cloud.CloudBillingMonitoring;

using AtlasOps.Features;

public sealed class CloudBillingMonitoringService(
    IAtlasOpsCapabilityRepository<CloudBillingMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudBillingMonitoringValidator validator = new();
    private readonly CloudBillingMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudBillingMonitoringChanged>> ExecuteAsync(
        UpdateCloudBillingMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudBillingMonitoringChanged>.Invalid(issues);
        }

        CloudBillingMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudBillingMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudBillingMonitoringChanged>.Invalid(
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

        CloudBillingMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudBillingMonitoringChanged>.Success(changed);
    }
}