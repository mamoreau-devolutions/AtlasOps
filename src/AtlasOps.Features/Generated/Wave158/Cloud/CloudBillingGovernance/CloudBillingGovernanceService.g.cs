namespace AtlasOps.Features.Cloud.CloudBillingGovernance;

using AtlasOps.Features;

public sealed class CloudBillingGovernanceService(
    IAtlasOpsCapabilityRepository<CloudBillingGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudBillingGovernanceValidator validator = new();
    private readonly CloudBillingGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudBillingGovernanceChanged>> ExecuteAsync(
        UpdateCloudBillingGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudBillingGovernanceChanged>.Invalid(issues);
        }

        CloudBillingGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudBillingGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudBillingGovernanceChanged>.Invalid(
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

        CloudBillingGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudBillingGovernanceChanged>.Success(changed);
    }
}