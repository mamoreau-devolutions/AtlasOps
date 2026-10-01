namespace AtlasOps.Features.Cloud.CloudNetworkGovernance;

using AtlasOps.Features;

public sealed class CloudNetworkGovernanceService(
    IAtlasOpsCapabilityRepository<CloudNetworkGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudNetworkGovernanceValidator validator = new();
    private readonly CloudNetworkGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudNetworkGovernanceChanged>> ExecuteAsync(
        UpdateCloudNetworkGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudNetworkGovernanceChanged>.Invalid(issues);
        }

        CloudNetworkGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudNetworkGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudNetworkGovernanceChanged>.Invalid(
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

        CloudNetworkGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudNetworkGovernanceChanged>.Success(changed);
    }
}