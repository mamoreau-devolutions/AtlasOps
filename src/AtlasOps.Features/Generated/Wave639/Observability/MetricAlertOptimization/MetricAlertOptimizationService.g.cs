namespace AtlasOps.Features.Observability.MetricAlertOptimization;

using AtlasOps.Features;

public sealed class MetricAlertOptimizationService(
    IAtlasOpsCapabilityRepository<MetricAlertOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricAlertOptimizationValidator validator = new();
    private readonly MetricAlertOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricAlertOptimizationChanged>> ExecuteAsync(
        UpdateMetricAlertOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricAlertOptimizationChanged>.Invalid(issues);
        }

        MetricAlertOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricAlertOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricAlertOptimizationChanged>.Invalid(
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

        MetricAlertOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricAlertOptimizationChanged>.Success(changed);
    }
}