namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupOptimization;

using AtlasOps.Features;

public sealed class RecoveryBackupOptimizationService(
    IAtlasOpsCapabilityRepository<RecoveryBackupOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryBackupOptimizationValidator validator = new();
    private readonly RecoveryBackupOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryBackupOptimizationChanged>> ExecuteAsync(
        UpdateRecoveryBackupOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryBackupOptimizationChanged>.Invalid(issues);
        }

        RecoveryBackupOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryBackupOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryBackupOptimizationChanged>.Invalid(
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

        RecoveryBackupOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryBackupOptimizationChanged>.Success(changed);
    }
}