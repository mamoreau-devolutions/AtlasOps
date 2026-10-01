namespace AtlasOps.Features.Data.DataAccessOptimization;

using AtlasOps.Features;

public sealed class DataAccessOptimizationService(
    IAtlasOpsCapabilityRepository<DataAccessOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataAccessOptimizationValidator validator = new();
    private readonly DataAccessOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataAccessOptimizationChanged>> ExecuteAsync(
        UpdateDataAccessOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataAccessOptimizationChanged>.Invalid(issues);
        }

        DataAccessOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataAccessOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataAccessOptimizationChanged>.Invalid(
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

        DataAccessOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataAccessOptimizationChanged>.Success(changed);
    }
}