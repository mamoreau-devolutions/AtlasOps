namespace AtlasOps.Features.Inventory.ReconciliationPlan;

using AtlasOps.Features;

public sealed class ReconciliationPlanService(
    IAtlasOpsCapabilityRepository<ReconciliationPlanItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReconciliationPlanValidator validator = new();
    private readonly ReconciliationPlanPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReconciliationPlanChanged>> ExecuteAsync(
        UpdateReconciliationPlanCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReconciliationPlanChanged>.Invalid(issues);
        }

        ReconciliationPlanItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReconciliationPlanItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReconciliationPlanChanged>.Invalid(
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

        ReconciliationPlanChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReconciliationPlanChanged>.Success(changed);
    }
}