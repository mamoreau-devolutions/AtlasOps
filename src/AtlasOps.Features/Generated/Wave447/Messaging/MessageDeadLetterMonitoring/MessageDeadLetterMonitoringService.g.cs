namespace AtlasOps.Features.Messaging.MessageDeadLetterMonitoring;

using AtlasOps.Features;

public sealed class MessageDeadLetterMonitoringService(
    IAtlasOpsCapabilityRepository<MessageDeadLetterMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageDeadLetterMonitoringValidator validator = new();
    private readonly MessageDeadLetterMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageDeadLetterMonitoringChanged>> ExecuteAsync(
        UpdateMessageDeadLetterMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageDeadLetterMonitoringChanged>.Invalid(issues);
        }

        MessageDeadLetterMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageDeadLetterMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageDeadLetterMonitoringChanged>.Invalid(
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

        MessageDeadLetterMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageDeadLetterMonitoringChanged>.Success(changed);
    }
}