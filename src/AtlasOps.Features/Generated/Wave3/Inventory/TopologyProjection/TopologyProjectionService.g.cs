namespace AtlasOps.Features.Inventory.TopologyProjection;

using AtlasOps.Features;

public sealed class TopologyProjectionService(
    IAtlasOpsCapabilityRepository<TopologyProjectionItem> repository,
    TimeProvider timeProvider)
{
    private readonly TopologyProjectionValidator validator = new();
    private readonly TopologyProjectionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TopologyProjectionChanged>> ExecuteAsync(
        UpdateTopologyProjectionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TopologyProjectionChanged>.Invalid(issues);
        }

        TopologyProjectionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TopologyProjectionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TopologyProjectionChanged>.Invalid(
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

        TopologyProjectionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TopologyProjectionChanged>.Success(changed);
    }
}