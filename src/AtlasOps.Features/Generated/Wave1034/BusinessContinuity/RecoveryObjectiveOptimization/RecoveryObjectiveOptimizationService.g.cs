namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveOptimization;

using AtlasOps.Features;

public sealed class RecoveryObjectiveOptimizationService(
    IAtlasOpsCapabilityRepository<RecoveryObjectiveOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryObjectiveOptimizationValidator validator = new();
    private readonly RecoveryObjectiveOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryObjectiveOptimizationChanged>> ExecuteAsync(
        UpdateRecoveryObjectiveOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryObjectiveOptimizationChanged>.Invalid(issues);
        }

        RecoveryObjectiveOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryObjectiveOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryObjectiveOptimizationChanged>.Invalid(
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

        RecoveryObjectiveOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryObjectiveOptimizationChanged>.Success(changed);
    }
}