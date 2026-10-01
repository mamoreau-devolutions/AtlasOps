namespace AtlasOps.Features.Network.NetworkVpnRecovery;

using AtlasOps.Features;

public sealed class NetworkVpnRecoveryService(
    IAtlasOpsCapabilityRepository<NetworkVpnRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkVpnRecoveryValidator validator = new();
    private readonly NetworkVpnRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkVpnRecoveryChanged>> ExecuteAsync(
        UpdateNetworkVpnRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkVpnRecoveryChanged>.Invalid(issues);
        }

        NetworkVpnRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkVpnRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkVpnRecoveryChanged>.Invalid(
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

        NetworkVpnRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkVpnRecoveryChanged>.Success(changed);
    }
}