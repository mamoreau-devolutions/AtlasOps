namespace AtlasOps.Features.Network.NetworkRouteGovernance;

using AtlasOps.Features;

public sealed class NetworkRouteGovernanceService(
    IAtlasOpsCapabilityRepository<NetworkRouteGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkRouteGovernanceValidator validator = new();
    private readonly NetworkRouteGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkRouteGovernanceChanged>> ExecuteAsync(
        UpdateNetworkRouteGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkRouteGovernanceChanged>.Invalid(issues);
        }

        NetworkRouteGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkRouteGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkRouteGovernanceChanged>.Invalid(
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

        NetworkRouteGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkRouteGovernanceChanged>.Success(changed);
    }
}