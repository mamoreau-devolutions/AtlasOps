namespace AtlasOps.Features.Compute.ComputeMetricOptimization;

using AtlasOps.Features;

public sealed class ComputeMetricOptimizationService(
    IAtlasOpsCapabilityRepository<ComputeMetricOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeMetricOptimizationValidator validator = new();
    private readonly ComputeMetricOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeMetricOptimizationChanged>> ExecuteAsync(
        UpdateComputeMetricOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeMetricOptimizationChanged>.Invalid(issues);
        }

        ComputeMetricOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeMetricOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeMetricOptimizationChanged>.Invalid(
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

        ComputeMetricOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeMetricOptimizationChanged>.Success(changed);
    }
}