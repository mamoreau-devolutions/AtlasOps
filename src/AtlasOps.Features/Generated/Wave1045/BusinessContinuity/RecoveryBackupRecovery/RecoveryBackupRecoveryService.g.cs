namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupRecovery;

using AtlasOps.Features;

public sealed class RecoveryBackupRecoveryService(
    IAtlasOpsCapabilityRepository<RecoveryBackupRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryBackupRecoveryValidator validator = new();
    private readonly RecoveryBackupRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryBackupRecoveryChanged>> ExecuteAsync(
        UpdateRecoveryBackupRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryBackupRecoveryChanged>.Invalid(issues);
        }

        RecoveryBackupRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryBackupRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryBackupRecoveryChanged>.Invalid(
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

        RecoveryBackupRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryBackupRecoveryChanged>.Success(changed);
    }
}