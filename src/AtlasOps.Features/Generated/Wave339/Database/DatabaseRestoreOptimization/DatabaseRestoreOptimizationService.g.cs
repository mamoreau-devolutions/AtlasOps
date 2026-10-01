namespace AtlasOps.Features.Database.DatabaseRestoreOptimization;

using AtlasOps.Features;

public sealed class DatabaseRestoreOptimizationService(
    IAtlasOpsCapabilityRepository<DatabaseRestoreOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseRestoreOptimizationValidator validator = new();
    private readonly DatabaseRestoreOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseRestoreOptimizationChanged>> ExecuteAsync(
        UpdateDatabaseRestoreOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseRestoreOptimizationChanged>.Invalid(issues);
        }

        DatabaseRestoreOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseRestoreOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseRestoreOptimizationChanged>.Invalid(
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

        DatabaseRestoreOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseRestoreOptimizationChanged>.Success(changed);
    }
}