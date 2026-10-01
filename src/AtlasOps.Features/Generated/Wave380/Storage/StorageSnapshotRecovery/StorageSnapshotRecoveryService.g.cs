namespace AtlasOps.Features.Storage.StorageSnapshotRecovery;

using AtlasOps.Features;

public sealed class StorageSnapshotRecoveryService(
    IAtlasOpsCapabilityRepository<StorageSnapshotRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageSnapshotRecoveryValidator validator = new();
    private readonly StorageSnapshotRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageSnapshotRecoveryChanged>> ExecuteAsync(
        UpdateStorageSnapshotRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageSnapshotRecoveryChanged>.Invalid(issues);
        }

        StorageSnapshotRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageSnapshotRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageSnapshotRecoveryChanged>.Invalid(
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

        StorageSnapshotRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageSnapshotRecoveryChanged>.Success(changed);
    }
}