namespace AtlasOps.Features.Data.DataTransformMonitoring;

using AtlasOps.Features;

public sealed class DataTransformMonitoringService(
    IAtlasOpsCapabilityRepository<DataTransformMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataTransformMonitoringValidator validator = new();
    private readonly DataTransformMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataTransformMonitoringChanged>> ExecuteAsync(
        UpdateDataTransformMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataTransformMonitoringChanged>.Invalid(issues);
        }

        DataTransformMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataTransformMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataTransformMonitoringChanged>.Invalid(
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

        DataTransformMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataTransformMonitoringChanged>.Success(changed);
    }
}