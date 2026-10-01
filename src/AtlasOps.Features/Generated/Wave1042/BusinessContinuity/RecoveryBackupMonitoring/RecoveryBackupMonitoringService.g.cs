namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupMonitoring;

using AtlasOps.Features;

public sealed class RecoveryBackupMonitoringService(
    IAtlasOpsCapabilityRepository<RecoveryBackupMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryBackupMonitoringValidator validator = new();
    private readonly RecoveryBackupMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryBackupMonitoringChanged>> ExecuteAsync(
        UpdateRecoveryBackupMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryBackupMonitoringChanged>.Invalid(issues);
        }

        RecoveryBackupMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryBackupMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryBackupMonitoringChanged>.Invalid(
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

        RecoveryBackupMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryBackupMonitoringChanged>.Success(changed);
    }
}