namespace AtlasOps.Features.Network.NetworkProbeRecovery;

using AtlasOps.Features;

public sealed class NetworkProbeRecoveryService(
    IAtlasOpsCapabilityRepository<NetworkProbeRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkProbeRecoveryValidator validator = new();
    private readonly NetworkProbeRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkProbeRecoveryChanged>> ExecuteAsync(
        UpdateNetworkProbeRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkProbeRecoveryChanged>.Invalid(issues);
        }

        NetworkProbeRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkProbeRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkProbeRecoveryChanged>.Invalid(
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

        NetworkProbeRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkProbeRecoveryChanged>.Success(changed);
    }
}