namespace AtlasOps.Features.Messaging.MessageBrokerOptimization;

using AtlasOps.Features;

public sealed class MessageBrokerOptimizationService(
    IAtlasOpsCapabilityRepository<MessageBrokerOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageBrokerOptimizationValidator validator = new();
    private readonly MessageBrokerOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageBrokerOptimizationChanged>> ExecuteAsync(
        UpdateMessageBrokerOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageBrokerOptimizationChanged>.Invalid(issues);
        }

        MessageBrokerOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageBrokerOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageBrokerOptimizationChanged>.Invalid(
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

        MessageBrokerOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageBrokerOptimizationChanged>.Success(changed);
    }
}