namespace AtlasOps.Features.Connections.RemoteClipboard;

using AtlasOps.Features;

public sealed class RemoteClipboardService(
    IAtlasOpsCapabilityRepository<RemoteClipboardItem> repository,
    TimeProvider timeProvider)
{
    private readonly RemoteClipboardValidator validator = new();
    private readonly RemoteClipboardPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RemoteClipboardChanged>> ExecuteAsync(
        UpdateRemoteClipboardCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RemoteClipboardChanged>.Invalid(issues);
        }

        RemoteClipboardItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RemoteClipboardItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RemoteClipboardChanged>.Invalid(
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

        RemoteClipboardChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RemoteClipboardChanged>.Success(changed);
    }
}