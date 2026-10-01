namespace AtlasOps.Features.Messaging.MessageTopicOptimization;

using AtlasOps.Features;

public sealed class MessageTopicOptimizationService(
    IAtlasOpsCapabilityRepository<MessageTopicOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageTopicOptimizationValidator validator = new();
    private readonly MessageTopicOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageTopicOptimizationChanged>> ExecuteAsync(
        UpdateMessageTopicOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageTopicOptimizationChanged>.Invalid(issues);
        }

        MessageTopicOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageTopicOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageTopicOptimizationChanged>.Invalid(
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

        MessageTopicOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageTopicOptimizationChanged>.Success(changed);
    }
}