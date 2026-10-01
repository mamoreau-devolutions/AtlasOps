namespace AtlasOps.Features.Network.NetworkSegmentRecovery;

using AtlasOps.Features;

public sealed class NetworkSegmentRecoveryService(
    IAtlasOpsCapabilityRepository<NetworkSegmentRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkSegmentRecoveryValidator validator = new();
    private readonly NetworkSegmentRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkSegmentRecoveryChanged>> ExecuteAsync(
        UpdateNetworkSegmentRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkSegmentRecoveryChanged>.Invalid(issues);
        }

        NetworkSegmentRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkSegmentRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkSegmentRecoveryChanged>.Invalid(
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

        NetworkSegmentRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkSegmentRecoveryChanged>.Success(changed);
    }
}