namespace AtlasOps.Features.Data.DataLineageMonitoring;

using AtlasOps.Features;

public sealed class DataLineageMonitoringService(
    IAtlasOpsCapabilityRepository<DataLineageMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataLineageMonitoringValidator validator = new();
    private readonly DataLineageMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataLineageMonitoringChanged>> ExecuteAsync(
        UpdateDataLineageMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataLineageMonitoringChanged>.Invalid(issues);
        }

        DataLineageMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataLineageMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataLineageMonitoringChanged>.Invalid(
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

        DataLineageMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataLineageMonitoringChanged>.Success(changed);
    }
}