namespace AtlasOps.Features.Database.DatabaseQueryOptimization;

using AtlasOps.Features;

public sealed class DatabaseQueryOptimizationService(
    IAtlasOpsCapabilityRepository<DatabaseQueryOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseQueryOptimizationValidator validator = new();
    private readonly DatabaseQueryOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseQueryOptimizationChanged>> ExecuteAsync(
        UpdateDatabaseQueryOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseQueryOptimizationChanged>.Invalid(issues);
        }

        DatabaseQueryOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseQueryOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseQueryOptimizationChanged>.Invalid(
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

        DatabaseQueryOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseQueryOptimizationChanged>.Success(changed);
    }
}