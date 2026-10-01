namespace AtlasOps.Features.Analytics.TopologyWidget;

using AtlasOps.Features;

public sealed class TopologyWidgetService(
    IAtlasOpsCapabilityRepository<TopologyWidgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly TopologyWidgetValidator validator = new();
    private readonly TopologyWidgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TopologyWidgetChanged>> ExecuteAsync(
        UpdateTopologyWidgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TopologyWidgetChanged>.Invalid(issues);
        }

        TopologyWidgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TopologyWidgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TopologyWidgetChanged>.Invalid(
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

        TopologyWidgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TopologyWidgetChanged>.Success(changed);
    }
}