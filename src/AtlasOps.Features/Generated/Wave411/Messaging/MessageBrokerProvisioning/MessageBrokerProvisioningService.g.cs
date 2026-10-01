namespace AtlasOps.Features.Messaging.MessageBrokerProvisioning;

using AtlasOps.Features;

public sealed class MessageBrokerProvisioningService(
    IAtlasOpsCapabilityRepository<MessageBrokerProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageBrokerProvisioningValidator validator = new();
    private readonly MessageBrokerProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageBrokerProvisioningChanged>> ExecuteAsync(
        UpdateMessageBrokerProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageBrokerProvisioningChanged>.Invalid(issues);
        }

        MessageBrokerProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageBrokerProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageBrokerProvisioningChanged>.Invalid(
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

        MessageBrokerProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageBrokerProvisioningChanged>.Success(changed);
    }
}