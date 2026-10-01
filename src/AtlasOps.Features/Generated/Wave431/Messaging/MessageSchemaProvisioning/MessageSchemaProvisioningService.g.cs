namespace AtlasOps.Features.Messaging.MessageSchemaProvisioning;

using AtlasOps.Features;

public sealed class MessageSchemaProvisioningService(
    IAtlasOpsCapabilityRepository<MessageSchemaProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageSchemaProvisioningValidator validator = new();
    private readonly MessageSchemaProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageSchemaProvisioningChanged>> ExecuteAsync(
        UpdateMessageSchemaProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageSchemaProvisioningChanged>.Invalid(issues);
        }

        MessageSchemaProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageSchemaProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageSchemaProvisioningChanged>.Invalid(
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

        MessageSchemaProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageSchemaProvisioningChanged>.Success(changed);
    }
}