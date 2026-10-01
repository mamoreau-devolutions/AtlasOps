namespace AtlasOps.Features.Storage.StorageEncryptionRecovery;

using AtlasOps.Features;

public sealed class StorageEncryptionRecoveryService(
    IAtlasOpsCapabilityRepository<StorageEncryptionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageEncryptionRecoveryValidator validator = new();
    private readonly StorageEncryptionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageEncryptionRecoveryChanged>> ExecuteAsync(
        UpdateStorageEncryptionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageEncryptionRecoveryChanged>.Invalid(issues);
        }

        StorageEncryptionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageEncryptionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageEncryptionRecoveryChanged>.Invalid(
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

        StorageEncryptionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageEncryptionRecoveryChanged>.Success(changed);
    }
}