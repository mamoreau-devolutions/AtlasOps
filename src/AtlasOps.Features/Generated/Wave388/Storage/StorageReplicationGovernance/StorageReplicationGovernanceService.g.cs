namespace AtlasOps.Features.Storage.StorageReplicationGovernance;

using AtlasOps.Features;

public sealed class StorageReplicationGovernanceService(
    IAtlasOpsCapabilityRepository<StorageReplicationGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageReplicationGovernanceValidator validator = new();
    private readonly StorageReplicationGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageReplicationGovernanceChanged>> ExecuteAsync(
        UpdateStorageReplicationGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageReplicationGovernanceChanged>.Invalid(issues);
        }

        StorageReplicationGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageReplicationGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageReplicationGovernanceChanged>.Invalid(
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

        StorageReplicationGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageReplicationGovernanceChanged>.Success(changed);
    }
}