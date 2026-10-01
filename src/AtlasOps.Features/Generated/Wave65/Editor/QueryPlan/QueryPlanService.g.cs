namespace AtlasOps.Features.Editor.QueryPlan;

using AtlasOps.Features;

public sealed class QueryPlanService(
    IAtlasOpsCapabilityRepository<QueryPlanItem> repository,
    TimeProvider timeProvider)
{
    private readonly QueryPlanValidator validator = new();
    private readonly QueryPlanPolicy policy = new();

    public async Task<AtlasOpsOperationResult<QueryPlanChanged>> ExecuteAsync(
        UpdateQueryPlanCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<QueryPlanChanged>.Invalid(issues);
        }

        QueryPlanItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new QueryPlanItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<QueryPlanChanged>.Invalid(
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

        QueryPlanChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<QueryPlanChanged>.Success(changed);
    }
}