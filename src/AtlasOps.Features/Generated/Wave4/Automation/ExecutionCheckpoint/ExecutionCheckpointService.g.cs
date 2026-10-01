namespace AtlasOps.Features.Automation.ExecutionCheckpoint;

using AtlasOps.Features;

public sealed class ExecutionCheckpointService(
    IAtlasOpsCapabilityRepository<ExecutionCheckpointItem> repository,
    TimeProvider timeProvider)
{
    private readonly ExecutionCheckpointValidator validator = new();
    private readonly ExecutionCheckpointPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ExecutionCheckpointChanged>> ExecuteAsync(
        UpdateExecutionCheckpointCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ExecutionCheckpointChanged>.Invalid(issues);
        }

        ExecutionCheckpointItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ExecutionCheckpointItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ExecutionCheckpointChanged>.Invalid(
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

        ExecutionCheckpointChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ExecutionCheckpointChanged>.Success(changed);
    }
}