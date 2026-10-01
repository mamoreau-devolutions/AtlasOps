namespace AtlasOps.Features.Storage.StorageLifecycleRecovery;

using AtlasOps.Features;

public sealed class StorageLifecycleRecoveryService(
    IAtlasOpsCapabilityRepository<StorageLifecycleRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageLifecycleRecoveryValidator validator = new();
    private readonly StorageLifecycleRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageLifecycleRecoveryChanged>> ExecuteAsync(
        UpdateStorageLifecycleRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageLifecycleRecoveryChanged>.Invalid(issues);
        }

        StorageLifecycleRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageLifecycleRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageLifecycleRecoveryChanged>.Invalid(
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

        StorageLifecycleRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageLifecycleRecoveryChanged>.Success(changed);
    }
}