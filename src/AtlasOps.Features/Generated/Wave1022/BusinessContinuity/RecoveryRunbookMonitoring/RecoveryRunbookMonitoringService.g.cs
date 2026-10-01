namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookMonitoring;

using AtlasOps.Features;

public sealed class RecoveryRunbookMonitoringService(
    IAtlasOpsCapabilityRepository<RecoveryRunbookMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryRunbookMonitoringValidator validator = new();
    private readonly RecoveryRunbookMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryRunbookMonitoringChanged>> ExecuteAsync(
        UpdateRecoveryRunbookMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryRunbookMonitoringChanged>.Invalid(issues);
        }

        RecoveryRunbookMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryRunbookMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryRunbookMonitoringChanged>.Invalid(
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

        RecoveryRunbookMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryRunbookMonitoringChanged>.Success(changed);
    }
}