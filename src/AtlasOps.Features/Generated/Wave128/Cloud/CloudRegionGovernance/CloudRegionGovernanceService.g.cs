namespace AtlasOps.Features.Cloud.CloudRegionGovernance;

using AtlasOps.Features;

public sealed class CloudRegionGovernanceService(
    IAtlasOpsCapabilityRepository<CloudRegionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudRegionGovernanceValidator validator = new();
    private readonly CloudRegionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudRegionGovernanceChanged>> ExecuteAsync(
        UpdateCloudRegionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudRegionGovernanceChanged>.Invalid(issues);
        }

        CloudRegionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudRegionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudRegionGovernanceChanged>.Invalid(
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

        CloudRegionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudRegionGovernanceChanged>.Success(changed);
    }
}