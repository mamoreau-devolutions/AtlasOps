namespace AtlasOps.Features.Editor.ResultProjection;

using AtlasOps.Features;

public sealed class ResultProjectionService(
    IAtlasOpsCapabilityRepository<ResultProjectionItem> repository,
    TimeProvider timeProvider)
{
    private readonly ResultProjectionValidator validator = new();
    private readonly ResultProjectionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ResultProjectionChanged>> ExecuteAsync(
        UpdateResultProjectionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ResultProjectionChanged>.Invalid(issues);
        }

        ResultProjectionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ResultProjectionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ResultProjectionChanged>.Invalid(
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

        ResultProjectionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ResultProjectionChanged>.Success(changed);
    }
}