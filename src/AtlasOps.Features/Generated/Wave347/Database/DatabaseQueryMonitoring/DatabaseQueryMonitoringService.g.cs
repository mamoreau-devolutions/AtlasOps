namespace AtlasOps.Features.Database.DatabaseQueryMonitoring;

using AtlasOps.Features;

public sealed class DatabaseQueryMonitoringService(
    IAtlasOpsCapabilityRepository<DatabaseQueryMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseQueryMonitoringValidator validator = new();
    private readonly DatabaseQueryMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseQueryMonitoringChanged>> ExecuteAsync(
        UpdateDatabaseQueryMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseQueryMonitoringChanged>.Invalid(issues);
        }

        DatabaseQueryMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseQueryMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseQueryMonitoringChanged>.Invalid(
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

        DatabaseQueryMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseQueryMonitoringChanged>.Success(changed);
    }
}