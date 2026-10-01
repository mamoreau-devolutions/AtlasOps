namespace AtlasOps.Features.Storage.StorageArchiveRecovery;

using AtlasOps.Features;

public sealed class StorageArchiveRecoveryService(
    IAtlasOpsCapabilityRepository<StorageArchiveRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageArchiveRecoveryValidator validator = new();
    private readonly StorageArchiveRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageArchiveRecoveryChanged>> ExecuteAsync(
        UpdateStorageArchiveRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageArchiveRecoveryChanged>.Invalid(issues);
        }

        StorageArchiveRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageArchiveRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageArchiveRecoveryChanged>.Invalid(
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

        StorageArchiveRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageArchiveRecoveryChanged>.Success(changed);
    }
}