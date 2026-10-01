namespace AtlasOps.Features.Incidents.IncidentActionItem;

using AtlasOps.Features;

public sealed class IncidentActionItemService(
    IAtlasOpsCapabilityRepository<IncidentActionItemItem> repository,
    TimeProvider timeProvider)
{
    private readonly IncidentActionItemValidator validator = new();
    private readonly IncidentActionItemPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IncidentActionItemChanged>> ExecuteAsync(
        UpdateIncidentActionItemCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IncidentActionItemChanged>.Invalid(issues);
        }

        IncidentActionItemItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IncidentActionItemItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IncidentActionItemChanged>.Invalid(
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

        IncidentActionItemChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IncidentActionItemChanged>.Success(changed);
    }
}