namespace AtlasOps.Features.Database.DatabaseRestoreMonitoring;

using AtlasOps.Features;

public sealed class DatabaseRestoreMonitoringService(
    IAtlasOpsCapabilityRepository<DatabaseRestoreMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseRestoreMonitoringValidator validator = new();
    private readonly DatabaseRestoreMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseRestoreMonitoringChanged>> ExecuteAsync(
        UpdateDatabaseRestoreMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseRestoreMonitoringChanged>.Invalid(issues);
        }

        DatabaseRestoreMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseRestoreMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseRestoreMonitoringChanged>.Invalid(
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

        DatabaseRestoreMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseRestoreMonitoringChanged>.Success(changed);
    }
}