namespace AtlasOps.Features.Cloud.CloudStorageGovernance;

using AtlasOps.Features;

public sealed class CloudStorageGovernanceService(
    IAtlasOpsCapabilityRepository<CloudStorageGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudStorageGovernanceValidator validator = new();
    private readonly CloudStorageGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudStorageGovernanceChanged>> ExecuteAsync(
        UpdateCloudStorageGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudStorageGovernanceChanged>.Invalid(issues);
        }

        CloudStorageGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudStorageGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudStorageGovernanceChanged>.Invalid(
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

        CloudStorageGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudStorageGovernanceChanged>.Success(changed);
    }
}