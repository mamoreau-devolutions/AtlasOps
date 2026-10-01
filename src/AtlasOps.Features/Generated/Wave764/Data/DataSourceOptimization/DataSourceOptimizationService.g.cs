namespace AtlasOps.Features.Data.DataSourceOptimization;

using AtlasOps.Features;

public sealed class DataSourceOptimizationService(
    IAtlasOpsCapabilityRepository<DataSourceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataSourceOptimizationValidator validator = new();
    private readonly DataSourceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataSourceOptimizationChanged>> ExecuteAsync(
        UpdateDataSourceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataSourceOptimizationChanged>.Invalid(issues);
        }

        DataSourceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataSourceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataSourceOptimizationChanged>.Invalid(
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

        DataSourceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataSourceOptimizationChanged>.Success(changed);
    }
}