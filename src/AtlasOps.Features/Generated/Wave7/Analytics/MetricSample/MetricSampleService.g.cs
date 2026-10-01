namespace AtlasOps.Features.Analytics.MetricSample;

using AtlasOps.Features;

public sealed class MetricSampleService(
    IAtlasOpsCapabilityRepository<MetricSampleItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricSampleValidator validator = new();
    private readonly MetricSamplePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricSampleChanged>> ExecuteAsync(
        UpdateMetricSampleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricSampleChanged>.Invalid(issues);
        }

        MetricSampleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricSampleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricSampleChanged>.Invalid(
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

        MetricSampleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricSampleChanged>.Success(changed);
    }
}