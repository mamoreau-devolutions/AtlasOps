namespace AtlasOps.Features.Analytics.MetricRetention;

using AtlasOps.Features;

public sealed class MetricRetentionService(
    IAtlasOpsCapabilityRepository<MetricRetentionItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricRetentionValidator validator = new();
    private readonly MetricRetentionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricRetentionChanged>> ExecuteAsync(
        UpdateMetricRetentionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricRetentionChanged>.Invalid(issues);
        }

        MetricRetentionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricRetentionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricRetentionChanged>.Invalid(
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

        MetricRetentionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricRetentionChanged>.Success(changed);
    }
}