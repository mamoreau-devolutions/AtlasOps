namespace AtlasOps.Features.Network.NetworkDnsZoneRecovery;

using AtlasOps.Features;

public sealed class NetworkDnsZoneRecoveryService(
    IAtlasOpsCapabilityRepository<NetworkDnsZoneRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkDnsZoneRecoveryValidator validator = new();
    private readonly NetworkDnsZoneRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkDnsZoneRecoveryChanged>> ExecuteAsync(
        UpdateNetworkDnsZoneRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkDnsZoneRecoveryChanged>.Invalid(issues);
        }

        NetworkDnsZoneRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkDnsZoneRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkDnsZoneRecoveryChanged>.Invalid(
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

        NetworkDnsZoneRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkDnsZoneRecoveryChanged>.Success(changed);
    }
}