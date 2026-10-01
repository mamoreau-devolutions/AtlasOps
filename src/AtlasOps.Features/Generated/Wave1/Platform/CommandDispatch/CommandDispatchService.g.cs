namespace AtlasOps.Features.Platform.CommandDispatch;

using AtlasOps.Features;

public sealed class CommandDispatchService(
    IAtlasOpsCapabilityRepository<CommandDispatchItem> repository,
    TimeProvider timeProvider)
{
    private readonly CommandDispatchValidator validator = new();
    private readonly CommandDispatchPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CommandDispatchChanged>> ExecuteAsync(
        UpdateCommandDispatchCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CommandDispatchChanged>.Invalid(issues);
        }

        CommandDispatchItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CommandDispatchItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CommandDispatchChanged>.Invalid(
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

        CommandDispatchChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CommandDispatchChanged>.Success(changed);
    }
}