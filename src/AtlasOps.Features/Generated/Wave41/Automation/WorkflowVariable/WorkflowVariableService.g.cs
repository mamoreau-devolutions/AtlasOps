namespace AtlasOps.Features.Automation.WorkflowVariable;

using AtlasOps.Features;

public sealed class WorkflowVariableService(
    IAtlasOpsCapabilityRepository<WorkflowVariableItem> repository,
    TimeProvider timeProvider)
{
    private readonly WorkflowVariableValidator validator = new();
    private readonly WorkflowVariablePolicy policy = new();

    public async Task<AtlasOpsOperationResult<WorkflowVariableChanged>> ExecuteAsync(
        UpdateWorkflowVariableCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<WorkflowVariableChanged>.Invalid(issues);
        }

        WorkflowVariableItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new WorkflowVariableItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<WorkflowVariableChanged>.Invalid(
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

        WorkflowVariableChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<WorkflowVariableChanged>.Success(changed);
    }
}