namespace AtlasOps.Features.Sync.WorkspaceClone;

using AtlasOps.Features;

public sealed class WorkspaceCloneService(
    IAtlasOpsCapabilityRepository<WorkspaceCloneItem> repository,
    TimeProvider timeProvider)
{
    private readonly WorkspaceCloneValidator validator = new();
    private readonly WorkspaceClonePolicy policy = new();

    public async Task<AtlasOpsOperationResult<WorkspaceCloneChanged>> ExecuteAsync(
        UpdateWorkspaceCloneCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<WorkspaceCloneChanged>.Invalid(issues);
        }

        WorkspaceCloneItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new WorkspaceCloneItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<WorkspaceCloneChanged>.Invalid(
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

        WorkspaceCloneChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<WorkspaceCloneChanged>.Success(changed);
    }
}