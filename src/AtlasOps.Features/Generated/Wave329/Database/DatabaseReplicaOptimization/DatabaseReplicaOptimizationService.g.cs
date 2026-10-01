namespace AtlasOps.Features.Database.DatabaseReplicaOptimization;

using AtlasOps.Features;

public sealed class DatabaseReplicaOptimizationService(
    IAtlasOpsCapabilityRepository<DatabaseReplicaOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseReplicaOptimizationValidator validator = new();
    private readonly DatabaseReplicaOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseReplicaOptimizationChanged>> ExecuteAsync(
        UpdateDatabaseReplicaOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseReplicaOptimizationChanged>.Invalid(issues);
        }

        DatabaseReplicaOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseReplicaOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseReplicaOptimizationChanged>.Invalid(
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

        DatabaseReplicaOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseReplicaOptimizationChanged>.Success(changed);
    }
}