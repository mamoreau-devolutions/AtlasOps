namespace AtlasOps.Features.Cloud.CloudIdentityMonitoring;

using AtlasOps.Features;

public sealed class CloudIdentityMonitoringService(
    IAtlasOpsCapabilityRepository<CloudIdentityMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudIdentityMonitoringValidator validator = new();
    private readonly CloudIdentityMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudIdentityMonitoringChanged>> ExecuteAsync(
        UpdateCloudIdentityMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudIdentityMonitoringChanged>.Invalid(issues);
        }

        CloudIdentityMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudIdentityMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudIdentityMonitoringChanged>.Invalid(
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

        CloudIdentityMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudIdentityMonitoringChanged>.Success(changed);
    }
}