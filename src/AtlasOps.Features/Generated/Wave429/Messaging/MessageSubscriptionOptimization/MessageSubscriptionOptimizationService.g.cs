namespace AtlasOps.Features.Messaging.MessageSubscriptionOptimization;

using AtlasOps.Features;

public sealed class MessageSubscriptionOptimizationService(
    IAtlasOpsCapabilityRepository<MessageSubscriptionOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageSubscriptionOptimizationValidator validator = new();
    private readonly MessageSubscriptionOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageSubscriptionOptimizationChanged>> ExecuteAsync(
        UpdateMessageSubscriptionOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageSubscriptionOptimizationChanged>.Invalid(issues);
        }

        MessageSubscriptionOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageSubscriptionOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageSubscriptionOptimizationChanged>.Invalid(
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

        MessageSubscriptionOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageSubscriptionOptimizationChanged>.Success(changed);
    }
}