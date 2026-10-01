namespace AtlasOps.Features.Database.DatabaseBackupOptimization;

using AtlasOps.Features;

public sealed class DatabaseBackupOptimizationService(
    IAtlasOpsCapabilityRepository<DatabaseBackupOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseBackupOptimizationValidator validator = new();
    private readonly DatabaseBackupOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseBackupOptimizationChanged>> ExecuteAsync(
        UpdateDatabaseBackupOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseBackupOptimizationChanged>.Invalid(issues);
        }

        DatabaseBackupOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseBackupOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseBackupOptimizationChanged>.Invalid(
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

        DatabaseBackupOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseBackupOptimizationChanged>.Success(changed);
    }
}