namespace AtlasOps.Features.Messaging.MessageSchemaOptimization;

using AtlasOps.Features;

public sealed class MessageSchemaOptimizationService(
    IAtlasOpsCapabilityRepository<MessageSchemaOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageSchemaOptimizationValidator validator = new();
    private readonly MessageSchemaOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageSchemaOptimizationChanged>> ExecuteAsync(
        UpdateMessageSchemaOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageSchemaOptimizationChanged>.Invalid(issues);
        }

        MessageSchemaOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageSchemaOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageSchemaOptimizationChanged>.Invalid(
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

        MessageSchemaOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageSchemaOptimizationChanged>.Success(changed);
    }
}