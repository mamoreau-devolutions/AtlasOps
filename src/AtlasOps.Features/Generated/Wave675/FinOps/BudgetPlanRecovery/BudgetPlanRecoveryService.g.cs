namespace AtlasOps.Features.FinOps.BudgetPlanRecovery;

using AtlasOps.Features;

public sealed class BudgetPlanRecoveryService(
    IAtlasOpsCapabilityRepository<BudgetPlanRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly BudgetPlanRecoveryValidator validator = new();
    private readonly BudgetPlanRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BudgetPlanRecoveryChanged>> ExecuteAsync(
        UpdateBudgetPlanRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BudgetPlanRecoveryChanged>.Invalid(issues);
        }

        BudgetPlanRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BudgetPlanRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BudgetPlanRecoveryChanged>.Invalid(
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

        BudgetPlanRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BudgetPlanRecoveryChanged>.Success(changed);
    }
}