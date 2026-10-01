namespace AtlasOps.Features.Editor.WorkspaceRecovery;

using AtlasOps.Features;

public sealed class WorkspaceRecoveryService(
    IAtlasOpsCapabilityRepository<WorkspaceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly WorkspaceRecoveryValidator validator = new();
    private readonly WorkspaceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<WorkspaceRecoveryChanged>> ExecuteAsync(
        UpdateWorkspaceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<WorkspaceRecoveryChanged>.Invalid(issues);
        }

        WorkspaceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new WorkspaceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<WorkspaceRecoveryChanged>.Invalid(
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

        WorkspaceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<WorkspaceRecoveryChanged>.Success(changed);
    }
}