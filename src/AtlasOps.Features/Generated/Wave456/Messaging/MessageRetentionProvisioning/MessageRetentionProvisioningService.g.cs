namespace AtlasOps.Features.Messaging.MessageRetentionProvisioning;

using AtlasOps.Features;

public sealed class MessageRetentionProvisioningService(
    IAtlasOpsCapabilityRepository<MessageRetentionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageRetentionProvisioningValidator validator = new();
    private readonly MessageRetentionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageRetentionProvisioningChanged>> ExecuteAsync(
        UpdateMessageRetentionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageRetentionProvisioningChanged>.Invalid(issues);
        }

        MessageRetentionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageRetentionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageRetentionProvisioningChanged>.Invalid(
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

        MessageRetentionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageRetentionProvisioningChanged>.Success(changed);
    }
}