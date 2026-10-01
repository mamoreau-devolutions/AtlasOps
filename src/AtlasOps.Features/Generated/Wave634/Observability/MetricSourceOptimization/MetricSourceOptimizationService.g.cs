namespace AtlasOps.Features.Observability.MetricSourceOptimization;

using AtlasOps.Features;

public sealed class MetricSourceOptimizationService(
    IAtlasOpsCapabilityRepository<MetricSourceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricSourceOptimizationValidator validator = new();
    private readonly MetricSourceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricSourceOptimizationChanged>> ExecuteAsync(
        UpdateMetricSourceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricSourceOptimizationChanged>.Invalid(issues);
        }

        MetricSourceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricSourceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricSourceOptimizationChanged>.Invalid(
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

        MetricSourceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricSourceOptimizationChanged>.Success(changed);
    }
}