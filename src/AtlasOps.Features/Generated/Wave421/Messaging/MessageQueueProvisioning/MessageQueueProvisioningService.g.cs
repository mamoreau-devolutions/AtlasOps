namespace AtlasOps.Features.Messaging.MessageQueueProvisioning;

using AtlasOps.Features;

public sealed class MessageQueueProvisioningService(
    IAtlasOpsCapabilityRepository<MessageQueueProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageQueueProvisioningValidator validator = new();
    private readonly MessageQueueProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageQueueProvisioningChanged>> ExecuteAsync(
        UpdateMessageQueueProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageQueueProvisioningChanged>.Invalid(issues);
        }

        MessageQueueProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageQueueProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageQueueProvisioningChanged>.Invalid(
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

        MessageQueueProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageQueueProvisioningChanged>.Success(changed);
    }
}