namespace AtlasOps.Features.Automation.RunbookDefinition;

using AtlasOps.Features;

public sealed class RunbookDefinitionService(
    IAtlasOpsCapabilityRepository<RunbookDefinitionItem> repository,
    TimeProvider timeProvider)
{
    private readonly RunbookDefinitionValidator validator = new();
    private readonly RunbookDefinitionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RunbookDefinitionChanged>> ExecuteAsync(
        UpdateRunbookDefinitionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RunbookDefinitionChanged>.Invalid(issues);
        }

        RunbookDefinitionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RunbookDefinitionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RunbookDefinitionChanged>.Invalid(
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

        RunbookDefinitionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RunbookDefinitionChanged>.Success(changed);
    }
}