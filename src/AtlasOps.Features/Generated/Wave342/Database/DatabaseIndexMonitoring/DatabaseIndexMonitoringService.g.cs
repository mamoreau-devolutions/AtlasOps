namespace AtlasOps.Features.Database.DatabaseIndexMonitoring;

using AtlasOps.Features;

public sealed class DatabaseIndexMonitoringService(
    IAtlasOpsCapabilityRepository<DatabaseIndexMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseIndexMonitoringValidator validator = new();
    private readonly DatabaseIndexMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseIndexMonitoringChanged>> ExecuteAsync(
        UpdateDatabaseIndexMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseIndexMonitoringChanged>.Invalid(issues);
        }

        DatabaseIndexMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseIndexMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseIndexMonitoringChanged>.Invalid(
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

        DatabaseIndexMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseIndexMonitoringChanged>.Success(changed);
    }
}