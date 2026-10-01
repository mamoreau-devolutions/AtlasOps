namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewGovernance;

using AtlasOps.Features;

public sealed class RecoveryReviewGovernanceService(
    IAtlasOpsCapabilityRepository<RecoveryReviewGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryReviewGovernanceValidator validator = new();
    private readonly RecoveryReviewGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryReviewGovernanceChanged>> ExecuteAsync(
        UpdateRecoveryReviewGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryReviewGovernanceChanged>.Invalid(issues);
        }

        RecoveryReviewGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryReviewGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryReviewGovernanceChanged>.Invalid(
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

        RecoveryReviewGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryReviewGovernanceChanged>.Success(changed);
    }
}