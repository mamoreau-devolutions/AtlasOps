namespace AtlasOps.Features.Automation.WorkflowSimulation;

using AtlasOps.Features;

public sealed class WorkflowSimulationService(
    IAtlasOpsCapabilityRepository<WorkflowSimulationItem> repository,
    TimeProvider timeProvider)
{
    private readonly WorkflowSimulationValidator validator = new();
    private readonly WorkflowSimulationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<WorkflowSimulationChanged>> ExecuteAsync(
        UpdateWorkflowSimulationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<WorkflowSimulationChanged>.Invalid(issues);
        }

        WorkflowSimulationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new WorkflowSimulationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<WorkflowSimulationChanged>.Invalid(
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

        WorkflowSimulationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<WorkflowSimulationChanged>.Success(changed);
    }
}