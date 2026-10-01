namespace AtlasOps.Features.Storage.StorageReplicationRecovery;

using AtlasOps.Features;

public sealed class StorageReplicationRecoveryService(
    IAtlasOpsCapabilityRepository<StorageReplicationRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageReplicationRecoveryValidator validator = new();
    private readonly StorageReplicationRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageReplicationRecoveryChanged>> ExecuteAsync(
        UpdateStorageReplicationRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageReplicationRecoveryChanged>.Invalid(issues);
        }

        StorageReplicationRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageReplicationRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageReplicationRecoveryChanged>.Invalid(
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

        StorageReplicationRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageReplicationRecoveryChanged>.Success(changed);
    }
}