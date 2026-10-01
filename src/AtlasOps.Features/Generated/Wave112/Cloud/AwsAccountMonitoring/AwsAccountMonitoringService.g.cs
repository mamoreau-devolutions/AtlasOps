namespace AtlasOps.Features.Cloud.AwsAccountMonitoring;

using AtlasOps.Features;

public sealed class AwsAccountMonitoringService(
    IAtlasOpsCapabilityRepository<AwsAccountMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly AwsAccountMonitoringValidator validator = new();
    private readonly AwsAccountMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AwsAccountMonitoringChanged>> ExecuteAsync(
        UpdateAwsAccountMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AwsAccountMonitoringChanged>.Invalid(issues);
        }

        AwsAccountMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AwsAccountMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AwsAccountMonitoringChanged>.Invalid(
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

        AwsAccountMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AwsAccountMonitoringChanged>.Success(changed);
    }
}