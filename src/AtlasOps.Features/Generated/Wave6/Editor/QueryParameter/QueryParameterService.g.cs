namespace AtlasOps.Features.Editor.QueryParameter;

using AtlasOps.Features;

public sealed class QueryParameterService(
    IAtlasOpsCapabilityRepository<QueryParameterItem> repository,
    TimeProvider timeProvider)
{
    private readonly QueryParameterValidator validator = new();
    private readonly QueryParameterPolicy policy = new();

    public async Task<AtlasOpsOperationResult<QueryParameterChanged>> ExecuteAsync(
        UpdateQueryParameterCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<QueryParameterChanged>.Invalid(issues);
        }

        QueryParameterItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new QueryParameterItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<QueryParameterChanged>.Invalid(
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

        QueryParameterChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<QueryParameterChanged>.Success(changed);
    }
}