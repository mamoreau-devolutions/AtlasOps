namespace AtlasOps.Features.FinOps.BudgetPlanMonitoring;

using AtlasOps.Features;

public sealed class BudgetPlanMonitoringService(
    IAtlasOpsCapabilityRepository<BudgetPlanMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly BudgetPlanMonitoringValidator validator = new();
    private readonly BudgetPlanMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BudgetPlanMonitoringChanged>> ExecuteAsync(
        UpdateBudgetPlanMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BudgetPlanMonitoringChanged>.Invalid(issues);
        }

        BudgetPlanMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BudgetPlanMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BudgetPlanMonitoringChanged>.Invalid(
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

        BudgetPlanMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BudgetPlanMonitoringChanged>.Success(changed);
    }
}