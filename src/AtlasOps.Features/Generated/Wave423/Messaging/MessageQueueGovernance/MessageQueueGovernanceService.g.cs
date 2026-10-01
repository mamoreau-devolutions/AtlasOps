namespace AtlasOps.Features.Messaging.MessageQueueGovernance;

using AtlasOps.Features;

public sealed class MessageQueueGovernanceService(
    IAtlasOpsCapabilityRepository<MessageQueueGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageQueueGovernanceValidator validator = new();
    private readonly MessageQueueGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageQueueGovernanceChanged>> ExecuteAsync(
        UpdateMessageQueueGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageQueueGovernanceChanged>.Invalid(issues);
        }

        MessageQueueGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageQueueGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageQueueGovernanceChanged>.Invalid(
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

        MessageQueueGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageQueueGovernanceChanged>.Success(changed);
    }
}