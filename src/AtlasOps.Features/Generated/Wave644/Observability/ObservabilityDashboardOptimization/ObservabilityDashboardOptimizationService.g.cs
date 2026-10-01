namespace AtlasOps.Features.Observability.ObservabilityDashboardOptimization;

using AtlasOps.Features;

public sealed class ObservabilityDashboardOptimizationService(
    IAtlasOpsCapabilityRepository<ObservabilityDashboardOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityDashboardOptimizationValidator validator = new();
    private readonly ObservabilityDashboardOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityDashboardOptimizationChanged>> ExecuteAsync(
        UpdateObservabilityDashboardOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityDashboardOptimizationChanged>.Invalid(issues);
        }

        ObservabilityDashboardOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityDashboardOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityDashboardOptimizationChanged>.Invalid(
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

        ObservabilityDashboardOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityDashboardOptimizationChanged>.Success(changed);
    }
}