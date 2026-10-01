namespace AtlasOps.Features.Sync.ReplicationPeer;

using AtlasOps.Features;

public sealed class ReplicationPeerService(
    IAtlasOpsCapabilityRepository<ReplicationPeerItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReplicationPeerValidator validator = new();
    private readonly ReplicationPeerPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReplicationPeerChanged>> ExecuteAsync(
        UpdateReplicationPeerCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReplicationPeerChanged>.Invalid(issues);
        }

        ReplicationPeerItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReplicationPeerItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReplicationPeerChanged>.Invalid(
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

        ReplicationPeerChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReplicationPeerChanged>.Success(changed);
    }
}