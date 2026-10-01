namespace AtlasOps.Features.Messaging.MessageProducerProvisioning;

using AtlasOps.Features;

public sealed class MessageProducerProvisioningService(
    IAtlasOpsCapabilityRepository<MessageProducerProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageProducerProvisioningValidator validator = new();
    private readonly MessageProducerProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageProducerProvisioningChanged>> ExecuteAsync(
        UpdateMessageProducerProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageProducerProvisioningChanged>.Invalid(issues);
        }

        MessageProducerProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageProducerProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageProducerProvisioningChanged>.Invalid(
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

        MessageProducerProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageProducerProvisioningChanged>.Success(changed);
    }
}