namespace AtlasOps.Features.Messaging.MessageProducerGovernance;

using AtlasOps.Features;

public sealed class MessageProducerGovernanceService(
    IAtlasOpsCapabilityRepository<MessageProducerGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageProducerGovernanceValidator validator = new();
    private readonly MessageProducerGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageProducerGovernanceChanged>> ExecuteAsync(
        UpdateMessageProducerGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageProducerGovernanceChanged>.Invalid(issues);
        }

        MessageProducerGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageProducerGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageProducerGovernanceChanged>.Invalid(
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

        MessageProducerGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageProducerGovernanceChanged>.Success(changed);
    }
}