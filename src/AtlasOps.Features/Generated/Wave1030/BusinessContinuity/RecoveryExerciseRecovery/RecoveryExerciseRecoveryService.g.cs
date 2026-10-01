namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseRecovery;

using AtlasOps.Features;

public sealed class RecoveryExerciseRecoveryService(
    IAtlasOpsCapabilityRepository<RecoveryExerciseRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryExerciseRecoveryValidator validator = new();
    private readonly RecoveryExerciseRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryExerciseRecoveryChanged>> ExecuteAsync(
        UpdateRecoveryExerciseRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryExerciseRecoveryChanged>.Invalid(issues);
        }

        RecoveryExerciseRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryExerciseRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryExerciseRecoveryChanged>.Invalid(
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

        RecoveryExerciseRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryExerciseRecoveryChanged>.Success(changed);
    }
}