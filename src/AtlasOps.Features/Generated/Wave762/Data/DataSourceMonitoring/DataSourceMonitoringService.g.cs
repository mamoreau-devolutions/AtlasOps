namespace AtlasOps.Features.Data.DataSourceMonitoring;

using AtlasOps.Features;

public sealed class DataSourceMonitoringService(
    IAtlasOpsCapabilityRepository<DataSourceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataSourceMonitoringValidator validator = new();
    private readonly DataSourceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataSourceMonitoringChanged>> ExecuteAsync(
        UpdateDataSourceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataSourceMonitoringChanged>.Invalid(issues);
        }

        DataSourceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataSourceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataSourceMonitoringChanged>.Invalid(
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

        DataSourceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataSourceMonitoringChanged>.Success(changed);
    }
}