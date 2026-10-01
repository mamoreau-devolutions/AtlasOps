namespace AtlasOps.Features.Cloud.CloudFunctionGovernance;

using AtlasOps.Features;

public sealed class CloudFunctionGovernanceService(
    IAtlasOpsCapabilityRepository<CloudFunctionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudFunctionGovernanceValidator validator = new();
    private readonly CloudFunctionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudFunctionGovernanceChanged>> ExecuteAsync(
        UpdateCloudFunctionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudFunctionGovernanceChanged>.Invalid(issues);
        }

        CloudFunctionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudFunctionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudFunctionGovernanceChanged>.Invalid(
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

        CloudFunctionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudFunctionGovernanceChanged>.Success(changed);
    }
}