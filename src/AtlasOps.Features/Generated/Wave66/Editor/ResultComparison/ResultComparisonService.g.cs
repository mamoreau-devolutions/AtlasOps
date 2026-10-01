namespace AtlasOps.Features.Editor.ResultComparison;

using AtlasOps.Features;

public sealed class ResultComparisonService(
    IAtlasOpsCapabilityRepository<ResultComparisonItem> repository,
    TimeProvider timeProvider)
{
    private readonly ResultComparisonValidator validator = new();
    private readonly ResultComparisonPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ResultComparisonChanged>> ExecuteAsync(
        UpdateResultComparisonCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ResultComparisonChanged>.Invalid(issues);
        }

        ResultComparisonItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ResultComparisonItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ResultComparisonChanged>.Invalid(
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

        ResultComparisonChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ResultComparisonChanged>.Success(changed);
    }
}