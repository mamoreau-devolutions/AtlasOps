namespace AtlasOps.Features.Sync.DurableInbox;

using AtlasOps.Features;

public sealed class DurableInboxService(
    IAtlasOpsCapabilityRepository<DurableInboxItem> repository,
    TimeProvider timeProvider)
{
    private readonly DurableInboxValidator validator = new();
    private readonly DurableInboxPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DurableInboxChanged>> ExecuteAsync(
        UpdateDurableInboxCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DurableInboxChanged>.Invalid(issues);
        }

        DurableInboxItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DurableInboxItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DurableInboxChanged>.Invalid(
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

        DurableInboxChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DurableInboxChanged>.Success(changed);
    }
}