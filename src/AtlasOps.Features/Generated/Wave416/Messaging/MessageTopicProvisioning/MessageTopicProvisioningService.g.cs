namespace AtlasOps.Features.Messaging.MessageTopicProvisioning;

using AtlasOps.Features;

public sealed class MessageTopicProvisioningService(
    IAtlasOpsCapabilityRepository<MessageTopicProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageTopicProvisioningValidator validator = new();
    private readonly MessageTopicProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageTopicProvisioningChanged>> ExecuteAsync(
        UpdateMessageTopicProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageTopicProvisioningChanged>.Invalid(issues);
        }

        MessageTopicProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageTopicProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageTopicProvisioningChanged>.Invalid(
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

        MessageTopicProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageTopicProvisioningChanged>.Success(changed);
    }
}