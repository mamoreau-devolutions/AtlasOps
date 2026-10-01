namespace AtlasOps.Features.Messaging.MessageProducerOptimization;

using AtlasOps.Features;

public sealed class MessageProducerOptimizationService(
    IAtlasOpsCapabilityRepository<MessageProducerOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageProducerOptimizationValidator validator = new();
    private readonly MessageProducerOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageProducerOptimizationChanged>> ExecuteAsync(
        UpdateMessageProducerOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageProducerOptimizationChanged>.Invalid(issues);
        }

        MessageProducerOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageProducerOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageProducerOptimizationChanged>.Invalid(
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

        MessageProducerOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageProducerOptimizationChanged>.Success(changed);
    }
}