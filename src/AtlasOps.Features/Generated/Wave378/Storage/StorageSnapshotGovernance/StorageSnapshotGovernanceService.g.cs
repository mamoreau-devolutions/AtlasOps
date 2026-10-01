namespace AtlasOps.Features.Storage.StorageSnapshotGovernance;

using AtlasOps.Features;

public sealed class StorageSnapshotGovernanceService(
    IAtlasOpsCapabilityRepository<StorageSnapshotGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageSnapshotGovernanceValidator validator = new();
    private readonly StorageSnapshotGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageSnapshotGovernanceChanged>> ExecuteAsync(
        UpdateStorageSnapshotGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageSnapshotGovernanceChanged>.Invalid(issues);
        }

        StorageSnapshotGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageSnapshotGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageSnapshotGovernanceChanged>.Invalid(
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

        StorageSnapshotGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageSnapshotGovernanceChanged>.Success(changed);
    }
}