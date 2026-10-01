namespace AtlasOps.Features.Cloud.CloudIdentityGovernance;

using AtlasOps.Features;

public sealed class CloudIdentityGovernanceService(
    IAtlasOpsCapabilityRepository<CloudIdentityGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudIdentityGovernanceValidator validator = new();
    private readonly CloudIdentityGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudIdentityGovernanceChanged>> ExecuteAsync(
        UpdateCloudIdentityGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudIdentityGovernanceChanged>.Invalid(issues);
        }

        CloudIdentityGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudIdentityGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudIdentityGovernanceChanged>.Invalid(
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

        CloudIdentityGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudIdentityGovernanceChanged>.Success(changed);
    }
}