namespace AtlasOps.Features.Messaging.MessageSubscriptionProvisioning;

using AtlasOps.Features;

public sealed class MessageSubscriptionProvisioningService(
    IAtlasOpsCapabilityRepository<MessageSubscriptionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageSubscriptionProvisioningValidator validator = new();
    private readonly MessageSubscriptionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageSubscriptionProvisioningChanged>> ExecuteAsync(
        UpdateMessageSubscriptionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageSubscriptionProvisioningChanged>.Invalid(issues);
        }

        MessageSubscriptionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageSubscriptionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageSubscriptionProvisioningChanged>.Invalid(
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

        MessageSubscriptionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageSubscriptionProvisioningChanged>.Success(changed);
    }
}