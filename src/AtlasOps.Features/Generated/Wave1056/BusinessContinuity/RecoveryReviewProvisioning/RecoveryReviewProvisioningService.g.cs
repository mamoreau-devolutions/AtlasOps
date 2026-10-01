namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewProvisioning;

using AtlasOps.Features;

public sealed class RecoveryReviewProvisioningService(
    IAtlasOpsCapabilityRepository<RecoveryReviewProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryReviewProvisioningValidator validator = new();
    private readonly RecoveryReviewProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryReviewProvisioningChanged>> ExecuteAsync(
        UpdateRecoveryReviewProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryReviewProvisioningChanged>.Invalid(issues);
        }

        RecoveryReviewProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryReviewProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryReviewProvisioningChanged>.Invalid(
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

        RecoveryReviewProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryReviewProvisioningChanged>.Success(changed);
    }
}