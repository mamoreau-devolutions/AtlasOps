namespace AtlasOps.Features.Data.DataPipelineOptimization;

using AtlasOps.Features;

public sealed class DataPipelineOptimizationService(
    IAtlasOpsCapabilityRepository<DataPipelineOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataPipelineOptimizationValidator validator = new();
    private readonly DataPipelineOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataPipelineOptimizationChanged>> ExecuteAsync(
        UpdateDataPipelineOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataPipelineOptimizationChanged>.Invalid(issues);
        }

        DataPipelineOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataPipelineOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataPipelineOptimizationChanged>.Invalid(
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

        DataPipelineOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataPipelineOptimizationChanged>.Success(changed);
    }
}