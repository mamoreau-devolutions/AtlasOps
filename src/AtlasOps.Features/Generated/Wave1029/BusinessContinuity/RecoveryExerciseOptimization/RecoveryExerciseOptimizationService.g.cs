namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseOptimization;

using AtlasOps.Features;

public sealed class RecoveryExerciseOptimizationService(
    IAtlasOpsCapabilityRepository<RecoveryExerciseOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryExerciseOptimizationValidator validator = new();
    private readonly RecoveryExerciseOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryExerciseOptimizationChanged>> ExecuteAsync(
        UpdateRecoveryExerciseOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryExerciseOptimizationChanged>.Invalid(issues);
        }

        RecoveryExerciseOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryExerciseOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryExerciseOptimizationChanged>.Invalid(
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

        RecoveryExerciseOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryExerciseOptimizationChanged>.Success(changed);
    }
}