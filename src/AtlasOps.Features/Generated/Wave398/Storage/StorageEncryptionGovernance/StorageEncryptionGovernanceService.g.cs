namespace AtlasOps.Features.Storage.StorageEncryptionGovernance;

using AtlasOps.Features;

public sealed class StorageEncryptionGovernanceService(
    IAtlasOpsCapabilityRepository<StorageEncryptionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageEncryptionGovernanceValidator validator = new();
    private readonly StorageEncryptionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageEncryptionGovernanceChanged>> ExecuteAsync(
        UpdateStorageEncryptionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageEncryptionGovernanceChanged>.Invalid(issues);
        }

        StorageEncryptionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageEncryptionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageEncryptionGovernanceChanged>.Invalid(
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

        StorageEncryptionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageEncryptionGovernanceChanged>.Success(changed);
    }
}