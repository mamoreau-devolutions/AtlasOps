namespace AtlasOps.Features.Messaging.MessageReplayProvisioning;

using AtlasOps.Features;

public sealed class MessageReplayProvisioningService(
    IAtlasOpsCapabilityRepository<MessageReplayProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageReplayProvisioningValidator validator = new();
    private readonly MessageReplayProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageReplayProvisioningChanged>> ExecuteAsync(
        UpdateMessageReplayProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageReplayProvisioningChanged>.Invalid(issues);
        }

        MessageReplayProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageReplayProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageReplayProvisioningChanged>.Invalid(
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

        MessageReplayProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageReplayProvisioningChanged>.Success(changed);
    }
}