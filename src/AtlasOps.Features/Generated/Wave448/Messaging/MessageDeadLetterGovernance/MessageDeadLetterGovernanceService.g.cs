namespace AtlasOps.Features.Messaging.MessageDeadLetterGovernance;

using AtlasOps.Features;

public sealed class MessageDeadLetterGovernanceService(
    IAtlasOpsCapabilityRepository<MessageDeadLetterGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageDeadLetterGovernanceValidator validator = new();
    private readonly MessageDeadLetterGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageDeadLetterGovernanceChanged>> ExecuteAsync(
        UpdateMessageDeadLetterGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageDeadLetterGovernanceChanged>.Invalid(issues);
        }

        MessageDeadLetterGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageDeadLetterGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageDeadLetterGovernanceChanged>.Invalid(
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

        MessageDeadLetterGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageDeadLetterGovernanceChanged>.Success(changed);
    }
}