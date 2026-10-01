namespace AtlasOps.Features.Storage.StorageArchiveGovernance;

using AtlasOps.Features;

public sealed class StorageArchiveGovernanceService(
    IAtlasOpsCapabilityRepository<StorageArchiveGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly StorageArchiveGovernanceValidator validator = new();
    private readonly StorageArchiveGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<StorageArchiveGovernanceChanged>> ExecuteAsync(
        UpdateStorageArchiveGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StorageArchiveGovernanceChanged>.Invalid(issues);
        }

        StorageArchiveGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StorageArchiveGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StorageArchiveGovernanceChanged>.Invalid(
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

        StorageArchiveGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StorageArchiveGovernanceChanged>.Success(changed);
    }
}