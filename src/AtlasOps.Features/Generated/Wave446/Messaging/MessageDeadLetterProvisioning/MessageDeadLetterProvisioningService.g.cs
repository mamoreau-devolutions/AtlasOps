namespace AtlasOps.Features.Messaging.MessageDeadLetterProvisioning;

using AtlasOps.Features;

public sealed class MessageDeadLetterProvisioningService(
    IAtlasOpsCapabilityRepository<MessageDeadLetterProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageDeadLetterProvisioningValidator validator = new();
    private readonly MessageDeadLetterProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageDeadLetterProvisioningChanged>> ExecuteAsync(
        UpdateMessageDeadLetterProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageDeadLetterProvisioningChanged>.Invalid(issues);
        }

        MessageDeadLetterProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageDeadLetterProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageDeadLetterProvisioningChanged>.Invalid(
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

        MessageDeadLetterProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageDeadLetterProvisioningChanged>.Success(changed);
    }
}