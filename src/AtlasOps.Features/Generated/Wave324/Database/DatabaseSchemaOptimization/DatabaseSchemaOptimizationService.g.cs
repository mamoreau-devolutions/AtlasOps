namespace AtlasOps.Features.Database.DatabaseSchemaOptimization;

using AtlasOps.Features;

public sealed class DatabaseSchemaOptimizationService(
    IAtlasOpsCapabilityRepository<DatabaseSchemaOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseSchemaOptimizationValidator validator = new();
    private readonly DatabaseSchemaOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseSchemaOptimizationChanged>> ExecuteAsync(
        UpdateDatabaseSchemaOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseSchemaOptimizationChanged>.Invalid(issues);
        }

        DatabaseSchemaOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseSchemaOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseSchemaOptimizationChanged>.Invalid(
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

        DatabaseSchemaOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseSchemaOptimizationChanged>.Success(changed);
    }
}