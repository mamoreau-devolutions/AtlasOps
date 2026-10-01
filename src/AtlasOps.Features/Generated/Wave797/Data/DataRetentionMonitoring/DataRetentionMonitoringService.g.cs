namespace AtlasOps.Features.Data.DataRetentionMonitoring;

using AtlasOps.Features;

public sealed class DataRetentionMonitoringService(
    IAtlasOpsCapabilityRepository<DataRetentionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataRetentionMonitoringValidator validator = new();
    private readonly DataRetentionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataRetentionMonitoringChanged>> ExecuteAsync(
        UpdateDataRetentionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataRetentionMonitoringChanged>.Invalid(issues);
        }

        DataRetentionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataRetentionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataRetentionMonitoringChanged>.Invalid(
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

        DataRetentionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataRetentionMonitoringChanged>.Success(changed);
    }
}