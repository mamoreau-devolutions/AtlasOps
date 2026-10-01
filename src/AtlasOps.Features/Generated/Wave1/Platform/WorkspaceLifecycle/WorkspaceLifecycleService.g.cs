namespace AtlasOps.Features.Platform.WorkspaceLifecycle;

using AtlasOps.Features;

public sealed class WorkspaceLifecycleService(
    IAtlasOpsCapabilityRepository<WorkspaceLifecycleItem> repository,
    TimeProvider timeProvider)
{
    private readonly WorkspaceLifecycleValidator validator = new();
    private readonly WorkspaceLifecyclePolicy policy = new();

    public async Task<AtlasOpsOperationResult<WorkspaceLifecycleChanged>> ExecuteAsync(
        UpdateWorkspaceLifecycleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<WorkspaceLifecycleChanged>.Invalid(issues);
        }

        WorkspaceLifecycleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new WorkspaceLifecycleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<WorkspaceLifecycleChanged>.Invalid(
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

        WorkspaceLifecycleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<WorkspaceLifecycleChanged>.Success(changed);
    }
}