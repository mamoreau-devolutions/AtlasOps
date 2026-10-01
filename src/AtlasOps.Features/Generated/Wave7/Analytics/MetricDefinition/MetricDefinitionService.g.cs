namespace AtlasOps.Features.Analytics.MetricDefinition;

using AtlasOps.Features;

public sealed class MetricDefinitionService(
    IAtlasOpsCapabilityRepository<MetricDefinitionItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricDefinitionValidator validator = new();
    private readonly MetricDefinitionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricDefinitionChanged>> ExecuteAsync(
        UpdateMetricDefinitionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricDefinitionChanged>.Invalid(issues);
        }

        MetricDefinitionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricDefinitionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricDefinitionChanged>.Invalid(
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

        MetricDefinitionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricDefinitionChanged>.Success(changed);
    }
}