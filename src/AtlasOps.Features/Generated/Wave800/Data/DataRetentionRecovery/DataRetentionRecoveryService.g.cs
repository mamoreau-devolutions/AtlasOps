namespace AtlasOps.Features.Data.DataRetentionRecovery;

using AtlasOps.Features;

public sealed class DataRetentionRecoveryService(
    IAtlasOpsCapabilityRepository<DataRetentionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataRetentionRecoveryValidator validator = new();
    private readonly DataRetentionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataRetentionRecoveryChanged>> ExecuteAsync(
        UpdateDataRetentionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataRetentionRecoveryChanged>.Invalid(issues);
        }

        DataRetentionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataRetentionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataRetentionRecoveryChanged>.Invalid(
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

        DataRetentionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataRetentionRecoveryChanged>.Success(changed);
    }
}