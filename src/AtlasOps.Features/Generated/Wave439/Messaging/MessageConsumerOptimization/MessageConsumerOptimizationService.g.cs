namespace AtlasOps.Features.Messaging.MessageConsumerOptimization;

using AtlasOps.Features;

public sealed class MessageConsumerOptimizationService(
    IAtlasOpsCapabilityRepository<MessageConsumerOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageConsumerOptimizationValidator validator = new();
    private readonly MessageConsumerOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageConsumerOptimizationChanged>> ExecuteAsync(
        UpdateMessageConsumerOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageConsumerOptimizationChanged>.Invalid(issues);
        }

        MessageConsumerOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageConsumerOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageConsumerOptimizationChanged>.Invalid(
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

        MessageConsumerOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageConsumerOptimizationChanged>.Success(changed);
    }
}