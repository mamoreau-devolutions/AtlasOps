namespace AtlasOps.Features.Messaging.MessageReplayGovernance;

using AtlasOps.Features;

public sealed class MessageReplayGovernanceService(
    IAtlasOpsCapabilityRepository<MessageReplayGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageReplayGovernanceValidator validator = new();
    private readonly MessageReplayGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageReplayGovernanceChanged>> ExecuteAsync(
        UpdateMessageReplayGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageReplayGovernanceChanged>.Invalid(issues);
        }

        MessageReplayGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageReplayGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageReplayGovernanceChanged>.Invalid(
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

        MessageReplayGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageReplayGovernanceChanged>.Success(changed);
    }
}