namespace AtlasOps.Features.Editor.WorkspaceBookmark;

using AtlasOps.Features;

public sealed class WorkspaceBookmarkService(
    IAtlasOpsCapabilityRepository<WorkspaceBookmarkItem> repository,
    TimeProvider timeProvider)
{
    private readonly WorkspaceBookmarkValidator validator = new();
    private readonly WorkspaceBookmarkPolicy policy = new();

    public async Task<AtlasOpsOperationResult<WorkspaceBookmarkChanged>> ExecuteAsync(
        UpdateWorkspaceBookmarkCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<WorkspaceBookmarkChanged>.Invalid(issues);
        }

        WorkspaceBookmarkItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new WorkspaceBookmarkItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<WorkspaceBookmarkChanged>.Invalid(
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

        WorkspaceBookmarkChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<WorkspaceBookmarkChanged>.Success(changed);
    }
}