namespace AtlasOps.Features.Network.NetworkSegmentGovernance;

using AtlasOps.Features;

public sealed class NetworkSegmentGovernanceService(
    IAtlasOpsCapabilityRepository<NetworkSegmentGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkSegmentGovernanceValidator validator = new();
    private readonly NetworkSegmentGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkSegmentGovernanceChanged>> ExecuteAsync(
        UpdateNetworkSegmentGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkSegmentGovernanceChanged>.Invalid(issues);
        }

        NetworkSegmentGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkSegmentGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkSegmentGovernanceChanged>.Invalid(
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

        NetworkSegmentGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkSegmentGovernanceChanged>.Success(changed);
    }
}