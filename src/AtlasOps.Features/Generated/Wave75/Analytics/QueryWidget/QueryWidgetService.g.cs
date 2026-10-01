namespace AtlasOps.Features.Analytics.QueryWidget;

using AtlasOps.Features;

public sealed class QueryWidgetService(
    IAtlasOpsCapabilityRepository<QueryWidgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly QueryWidgetValidator validator = new();
    private readonly QueryWidgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<QueryWidgetChanged>> ExecuteAsync(
        UpdateQueryWidgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<QueryWidgetChanged>.Invalid(issues);
        }

        QueryWidgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new QueryWidgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<QueryWidgetChanged>.Invalid(
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

        QueryWidgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<QueryWidgetChanged>.Success(changed);
    }
}