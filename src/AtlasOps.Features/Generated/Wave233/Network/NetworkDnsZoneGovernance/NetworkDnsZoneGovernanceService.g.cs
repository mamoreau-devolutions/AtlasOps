namespace AtlasOps.Features.Network.NetworkDnsZoneGovernance;

using AtlasOps.Features;

public sealed class NetworkDnsZoneGovernanceService(
    IAtlasOpsCapabilityRepository<NetworkDnsZoneGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkDnsZoneGovernanceValidator validator = new();
    private readonly NetworkDnsZoneGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkDnsZoneGovernanceChanged>> ExecuteAsync(
        UpdateNetworkDnsZoneGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkDnsZoneGovernanceChanged>.Invalid(issues);
        }

        NetworkDnsZoneGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkDnsZoneGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkDnsZoneGovernanceChanged>.Invalid(
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

        NetworkDnsZoneGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkDnsZoneGovernanceChanged>.Success(changed);
    }
}