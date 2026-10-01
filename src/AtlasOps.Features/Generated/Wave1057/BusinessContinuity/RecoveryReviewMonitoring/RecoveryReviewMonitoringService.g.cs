namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewMonitoring;

using AtlasOps.Features;

public sealed class RecoveryReviewMonitoringService(
    IAtlasOpsCapabilityRepository<RecoveryReviewMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryReviewMonitoringValidator validator = new();
    private readonly RecoveryReviewMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryReviewMonitoringChanged>> ExecuteAsync(
        UpdateRecoveryReviewMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryReviewMonitoringChanged>.Invalid(issues);
        }

        RecoveryReviewMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryReviewMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryReviewMonitoringChanged>.Invalid(
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

        RecoveryReviewMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryReviewMonitoringChanged>.Success(changed);
    }
}