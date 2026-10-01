namespace AtlasOps.Features.Automation.RunbookExecution;

using AtlasOps.Features;

public sealed class RunbookExecutionService(
    IAtlasOpsCapabilityRepository<RunbookExecutionItem> repository,
    TimeProvider timeProvider)
{
    private readonly RunbookExecutionValidator validator = new();
    private readonly RunbookExecutionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RunbookExecutionChanged>> ExecuteAsync(
        UpdateRunbookExecutionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RunbookExecutionChanged>.Invalid(issues);
        }

        RunbookExecutionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RunbookExecutionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RunbookExecutionChanged>.Invalid(
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

        RunbookExecutionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RunbookExecutionChanged>.Success(changed);
    }
}