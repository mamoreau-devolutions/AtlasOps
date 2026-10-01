namespace AtlasOps.Features.Data.DataDatasetOptimization;

using AtlasOps.Features;

public sealed class DataDatasetOptimizationService(
    IAtlasOpsCapabilityRepository<DataDatasetOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataDatasetOptimizationValidator validator = new();
    private readonly DataDatasetOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataDatasetOptimizationChanged>> ExecuteAsync(
        UpdateDataDatasetOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataDatasetOptimizationChanged>.Invalid(issues);
        }

        DataDatasetOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataDatasetOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataDatasetOptimizationChanged>.Invalid(
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

        DataDatasetOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataDatasetOptimizationChanged>.Success(changed);
    }
}