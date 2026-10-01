namespace AtlasOps.Features.Network.NetworkPolicyRecovery;

using AtlasOps.Features;

public sealed class NetworkPolicyRecoveryService(
    IAtlasOpsCapabilityRepository<NetworkPolicyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkPolicyRecoveryValidator validator = new();
    private readonly NetworkPolicyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkPolicyRecoveryChanged>> ExecuteAsync(
        UpdateNetworkPolicyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkPolicyRecoveryChanged>.Invalid(issues);
        }

        NetworkPolicyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkPolicyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkPolicyRecoveryChanged>.Invalid(
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

        NetworkPolicyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkPolicyRecoveryChanged>.Success(changed);
    }
}