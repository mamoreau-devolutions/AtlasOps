namespace AtlasOps.Features.Analytics.MetricFormula;

using AtlasOps.Features;

public sealed class MetricFormulaService(
    IAtlasOpsCapabilityRepository<MetricFormulaItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricFormulaValidator validator = new();
    private readonly MetricFormulaPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricFormulaChanged>> ExecuteAsync(
        UpdateMetricFormulaCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricFormulaChanged>.Invalid(issues);
        }

        MetricFormulaItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricFormulaItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricFormulaChanged>.Invalid(
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

        MetricFormulaChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricFormulaChanged>.Success(changed);
    }
}