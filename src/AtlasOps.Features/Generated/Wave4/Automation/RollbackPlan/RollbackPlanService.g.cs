namespace AtlasOps.Features.Automation.RollbackPlan;

using AtlasOps.Features;

public sealed class RollbackPlanService(
    IAtlasOpsCapabilityRepository<RollbackPlanItem> repository,
    TimeProvider timeProvider)
{
    private readonly RollbackPlanValidator validator = new();
    private readonly RollbackPlanPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RollbackPlanChanged>> ExecuteAsync(
        UpdateRollbackPlanCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RollbackPlanChanged>.Invalid(issues);
        }

        RollbackPlanItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RollbackPlanItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RollbackPlanChanged>.Invalid(
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

        RollbackPlanChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RollbackPlanChanged>.Success(changed);
    }
}