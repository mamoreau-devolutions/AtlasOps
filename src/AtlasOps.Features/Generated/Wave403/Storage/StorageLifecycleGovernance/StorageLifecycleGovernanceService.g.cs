namespace AtlasOps.Features.Storage.StorageLifecycleGovernance;

using AtlasOps.Features;

public sealed class StorageLifecycleGovernanceService(
    IAtlasOpsCapabilityRepository<StorageLifecycleGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageLifecycleGovernanceValidator validator = new();
    private readonly StorageLifecycleGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageLifecycleGovernanceChanged>> ExecuteAsync(
        UpdateStorageLifecycleGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageLifecycleGovernanceChanged>.Invalid(issues);
        }

        StorageLifecycleGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageLifecycleGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageLifecycleGovernanceChanged>.Invalid(
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

        StorageLifecycleGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageLifecycleGovernanceChanged>.Success(changed);
    }
}