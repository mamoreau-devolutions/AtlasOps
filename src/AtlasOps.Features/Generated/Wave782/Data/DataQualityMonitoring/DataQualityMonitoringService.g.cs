namespace AtlasOps.Features.Data.DataQualityMonitoring;

using AtlasOps.Features;

public sealed class DataQualityMonitoringService(
    IAtlasOpsCapabilityRepository<DataQualityMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataQualityMonitoringValidator validator = new();
    private readonly DataQualityMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataQualityMonitoringChanged>> ExecuteAsync(
        UpdateDataQualityMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataQualityMonitoringChanged>.Invalid(issues);
        }

        DataQualityMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataQualityMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataQualityMonitoringChanged>.Invalid(
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

        DataQualityMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataQualityMonitoringChanged>.Success(changed);
    }
}