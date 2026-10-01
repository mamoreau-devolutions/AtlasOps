namespace AtlasOps.Features.Database.DatabaseReplicaMonitoring;

using AtlasOps.Features;

public sealed class DatabaseReplicaMonitoringService(
    IAtlasOpsCapabilityRepository<DatabaseReplicaMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseReplicaMonitoringValidator validator = new();
    private readonly DatabaseReplicaMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseReplicaMonitoringChanged>> ExecuteAsync(
        UpdateDatabaseReplicaMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseReplicaMonitoringChanged>.Invalid(issues);
        }

        DatabaseReplicaMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseReplicaMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseReplicaMonitoringChanged>.Invalid(
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

        DatabaseReplicaMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseReplicaMonitoringChanged>.Success(changed);
    }
}