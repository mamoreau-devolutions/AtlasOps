namespace AtlasOps.Features.Storage.BlockVolumeRecovery;

using AtlasOps.Features;

public sealed class BlockVolumeRecoveryService(
    IAtlasOpsCapabilityRepository<BlockVolumeRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly BlockVolumeRecoveryValidator validator = new();
    private readonly BlockVolumeRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BlockVolumeRecoveryChanged>> ExecuteAsync(
        UpdateBlockVolumeRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BlockVolumeRecoveryChanged>.Invalid(issues);
        }

        BlockVolumeRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BlockVolumeRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BlockVolumeRecoveryChanged>.Invalid(
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

        BlockVolumeRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BlockVolumeRecoveryChanged>.Success(changed);
    }
}