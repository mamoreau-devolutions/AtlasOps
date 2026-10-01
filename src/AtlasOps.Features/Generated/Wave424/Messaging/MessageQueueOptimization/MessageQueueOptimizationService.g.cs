namespace AtlasOps.Features.Messaging.MessageQueueOptimization;

using AtlasOps.Features;

public sealed class MessageQueueOptimizationService(
    IAtlasOpsCapabilityRepository<MessageQueueOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageQueueOptimizationValidator validator = new();
    private readonly MessageQueueOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageQueueOptimizationChanged>> ExecuteAsync(
        UpdateMessageQueueOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageQueueOptimizationChanged>.Invalid(issues);
        }

        MessageQueueOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageQueueOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageQueueOptimizationChanged>.Invalid(
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

        MessageQueueOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageQueueOptimizationChanged>.Success(changed);
    }
}