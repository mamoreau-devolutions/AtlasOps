namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseMonitoring;

using AtlasOps.Features;

public sealed class RecoveryExerciseMonitoringService(
    IAtlasOpsCapabilityRepository<RecoveryExerciseMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryExerciseMonitoringValidator validator = new();
    private readonly RecoveryExerciseMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryExerciseMonitoringChanged>> ExecuteAsync(
        UpdateRecoveryExerciseMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryExerciseMonitoringChanged>.Invalid(issues);
        }

        RecoveryExerciseMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryExerciseMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryExerciseMonitoringChanged>.Invalid(
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

        RecoveryExerciseMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryExerciseMonitoringChanged>.Success(changed);
    }
}