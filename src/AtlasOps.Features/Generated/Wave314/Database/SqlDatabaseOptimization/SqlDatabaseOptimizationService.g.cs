namespace AtlasOps.Features.Database.SqlDatabaseOptimization;

using AtlasOps.Features;

public sealed class SqlDatabaseOptimizationService(
    IAtlasOpsCapabilityRepository<SqlDatabaseOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SqlDatabaseOptimizationValidator validator = new();
    private readonly SqlDatabaseOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SqlDatabaseOptimizationChanged>> ExecuteAsync(
        UpdateSqlDatabaseOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SqlDatabaseOptimizationChanged>.Invalid(issues);
        }

        SqlDatabaseOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SqlDatabaseOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SqlDatabaseOptimizationChanged>.Invalid(
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

        SqlDatabaseOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SqlDatabaseOptimizationChanged>.Success(changed);
    }
}