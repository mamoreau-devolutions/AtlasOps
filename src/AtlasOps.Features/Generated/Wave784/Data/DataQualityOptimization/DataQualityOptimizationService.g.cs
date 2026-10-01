namespace AtlasOps.Features.Data.DataQualityOptimization;

using AtlasOps.Features;

public sealed class DataQualityOptimizationService(
    IAtlasOpsCapabilityRepository<DataQualityOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataQualityOptimizationValidator validator = new();
    private readonly DataQualityOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataQualityOptimizationChanged>> ExecuteAsync(
        UpdateDataQualityOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataQualityOptimizationChanged>.Invalid(issues);
        }

        DataQualityOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataQualityOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataQualityOptimizationChanged>.Invalid(
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

        DataQualityOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataQualityOptimizationChanged>.Success(changed);
    }
}