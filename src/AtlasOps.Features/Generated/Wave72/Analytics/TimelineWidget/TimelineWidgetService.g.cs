namespace AtlasOps.Features.Analytics.TimelineWidget;

using AtlasOps.Features;

public sealed class TimelineWidgetService(
    IAtlasOpsCapabilityRepository<TimelineWidgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly TimelineWidgetValidator validator = new();
    private readonly TimelineWidgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TimelineWidgetChanged>> ExecuteAsync(
        UpdateTimelineWidgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TimelineWidgetChanged>.Invalid(issues);
        }

        TimelineWidgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TimelineWidgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TimelineWidgetChanged>.Invalid(
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

        TimelineWidgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TimelineWidgetChanged>.Success(changed);
    }
}