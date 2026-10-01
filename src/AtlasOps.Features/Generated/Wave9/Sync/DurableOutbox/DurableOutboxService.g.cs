namespace AtlasOps.Features.Sync.DurableOutbox;

using AtlasOps.Features;

public sealed class DurableOutboxService(
    IAtlasOpsCapabilityRepository<DurableOutboxItem> repository,
    TimeProvider timeProvider)
{
    private readonly DurableOutboxValidator validator = new();
    private readonly DurableOutboxPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DurableOutboxChanged>> ExecuteAsync(
        UpdateDurableOutboxCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DurableOutboxChanged>.Invalid(issues);
        }

        DurableOutboxItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DurableOutboxItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DurableOutboxChanged>.Invalid(
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

        DurableOutboxChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DurableOutboxChanged>.Success(changed);
    }
}