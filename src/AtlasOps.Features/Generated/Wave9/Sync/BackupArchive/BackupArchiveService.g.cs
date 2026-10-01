namespace AtlasOps.Features.Sync.BackupArchive;

using AtlasOps.Features;

public sealed class BackupArchiveService(
    IAtlasOpsCapabilityRepository<BackupArchiveItem> repository,
    TimeProvider timeProvider)
{
    private readonly BackupArchiveValidator validator = new();
    private readonly BackupArchivePolicy policy = new();

    public async Task<AtlasOpsOperationResult<BackupArchiveChanged>> ExecuteAsync(
        UpdateBackupArchiveCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BackupArchiveChanged>.Invalid(issues);
        }

        BackupArchiveItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BackupArchiveItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BackupArchiveChanged>.Invalid(
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

        BackupArchiveChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BackupArchiveChanged>.Success(changed);
    }
}