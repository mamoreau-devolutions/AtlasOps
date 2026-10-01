namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewOptimization;

using AtlasOps.Features;

public sealed class RecoveryReviewOptimizationService(
    IAtlasOpsCapabilityRepository<RecoveryReviewOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryReviewOptimizationValidator validator = new();
    private readonly RecoveryReviewOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryReviewOptimizationChanged>> ExecuteAsync(
        UpdateRecoveryReviewOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryReviewOptimizationChanged>.Invalid(issues);
        }

        RecoveryReviewOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryReviewOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryReviewOptimizationChanged>.Invalid(
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

        RecoveryReviewOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryReviewOptimizationChanged>.Success(changed);
    }
}