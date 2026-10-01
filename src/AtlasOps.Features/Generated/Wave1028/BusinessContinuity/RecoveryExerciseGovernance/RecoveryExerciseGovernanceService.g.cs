namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseGovernance;

using AtlasOps.Features;

public sealed class RecoveryExerciseGovernanceService(
    IAtlasOpsCapabilityRepository<RecoveryExerciseGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryExerciseGovernanceValidator validator = new();
    private readonly RecoveryExerciseGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryExerciseGovernanceChanged>> ExecuteAsync(
        UpdateRecoveryExerciseGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryExerciseGovernanceChanged>.Invalid(issues);
        }

        RecoveryExerciseGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryExerciseGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryExerciseGovernanceChanged>.Invalid(
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

        RecoveryExerciseGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryExerciseGovernanceChanged>.Success(changed);
    }
}