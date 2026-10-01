namespace AtlasOps.Features.Messaging.MessageConsumerGovernance;

using AtlasOps.Features;

public sealed class MessageConsumerGovernanceService(
    IAtlasOpsCapabilityRepository<MessageConsumerGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageConsumerGovernanceValidator validator = new();
    private readonly MessageConsumerGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageConsumerGovernanceChanged>> ExecuteAsync(
        UpdateMessageConsumerGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageConsumerGovernanceChanged>.Invalid(issues);
        }

        MessageConsumerGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageConsumerGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageConsumerGovernanceChanged>.Invalid(
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

        MessageConsumerGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageConsumerGovernanceChanged>.Success(changed);
    }
}