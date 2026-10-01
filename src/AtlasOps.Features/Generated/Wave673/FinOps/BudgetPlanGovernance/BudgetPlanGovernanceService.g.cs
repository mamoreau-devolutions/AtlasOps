namespace AtlasOps.Features.FinOps.BudgetPlanGovernance;

using AtlasOps.Features;

public sealed class BudgetPlanGovernanceService(
    IAtlasOpsCapabilityRepository<BudgetPlanGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly BudgetPlanGovernanceValidator validator = new();
    private readonly BudgetPlanGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<BudgetPlanGovernanceChanged>> ExecuteAsync(
        UpdateBudgetPlanGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BudgetPlanGovernanceChanged>.Invalid(issues);
        }

        BudgetPlanGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BudgetPlanGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BudgetPlanGovernanceChanged>.Invalid(
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

        BudgetPlanGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BudgetPlanGovernanceChanged>.Success(changed);
    }
}