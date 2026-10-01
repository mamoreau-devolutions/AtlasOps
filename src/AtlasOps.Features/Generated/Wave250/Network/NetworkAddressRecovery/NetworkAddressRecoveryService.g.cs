namespace AtlasOps.Features.Network.NetworkAddressRecovery;

using AtlasOps.Features;

public sealed class NetworkAddressRecoveryService(
    IAtlasOpsCapabilityRepository<NetworkAddressRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkAddressRecoveryValidator validator = new();
    private readonly NetworkAddressRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkAddressRecoveryChanged>> ExecuteAsync(
        UpdateNetworkAddressRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkAddressRecoveryChanged>.Invalid(issues);
        }

        NetworkAddressRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkAddressRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkAddressRecoveryChanged>.Invalid(
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

        NetworkAddressRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkAddressRecoveryChanged>.Success(changed);
    }
}