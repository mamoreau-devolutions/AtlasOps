namespace AtlasOps.Features.Cloud.CloudFunctionMonitoring;

using AtlasOps.Features;

public sealed class CloudFunctionMonitoringService(
    IAtlasOpsCapabilityRepository<CloudFunctionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudFunctionMonitoringValidator validator = new();
    private readonly CloudFunctionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudFunctionMonitoringChanged>> ExecuteAsync(
        UpdateCloudFunctionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudFunctionMonitoringChanged>.Invalid(issues);
        }

        CloudFunctionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudFunctionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudFunctionMonitoringChanged>.Invalid(
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

        CloudFunctionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudFunctionMonitoringChanged>.Success(changed);
    }
}