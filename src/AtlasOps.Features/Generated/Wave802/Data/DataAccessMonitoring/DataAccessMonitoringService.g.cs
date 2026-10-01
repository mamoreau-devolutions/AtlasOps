namespace AtlasOps.Features.Data.DataAccessMonitoring;

using AtlasOps.Features;

public sealed class DataAccessMonitoringService(
    IAtlasOpsCapabilityRepository<DataAccessMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataAccessMonitoringValidator validator = new();
    private readonly DataAccessMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataAccessMonitoringChanged>> ExecuteAsync(
        UpdateDataAccessMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataAccessMonitoringChanged>.Invalid(issues);
        }

        DataAccessMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataAccessMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataAccessMonitoringChanged>.Invalid(
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

        DataAccessMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataAccessMonitoringChanged>.Success(changed);
    }
}