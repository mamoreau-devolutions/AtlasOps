namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewRecovery;

using AtlasOps.Features;

public sealed class RecoveryReviewRecoveryService(
    IAtlasOpsCapabilityRepository<RecoveryReviewRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryReviewRecoveryValidator validator = new();
    private readonly RecoveryReviewRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryReviewRecoveryChanged>> ExecuteAsync(
        UpdateRecoveryReviewRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryReviewRecoveryChanged>.Invalid(issues);
        }

        RecoveryReviewRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryReviewRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryReviewRecoveryChanged>.Invalid(
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

        RecoveryReviewRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryReviewRecoveryChanged>.Success(changed);
    }
}