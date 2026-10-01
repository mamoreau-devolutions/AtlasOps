namespace AtlasOps.Features.Messaging.MessageProducerRecovery;

using AtlasOps.Features;

public sealed class MessageProducerRecoveryService(
    IAtlasOpsCapabilityRepository<MessageProducerRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageProducerRecoveryValidator validator = new();
    private readonly MessageProducerRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageProducerRecoveryChanged>> ExecuteAsync(
        UpdateMessageProducerRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageProducerRecoveryChanged>.Invalid(issues);
        }

        MessageProducerRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageProducerRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageProducerRecoveryChanged>.Invalid(
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

        MessageProducerRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageProducerRecoveryChanged>.Success(changed);
    }
}