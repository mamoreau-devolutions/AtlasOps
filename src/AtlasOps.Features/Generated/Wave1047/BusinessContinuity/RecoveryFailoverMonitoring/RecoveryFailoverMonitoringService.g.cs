namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverMonitoring;

using AtlasOps.Features;

public sealed class RecoveryFailoverMonitoringService(
    IAtlasOpsCapabilityRepository<RecoveryFailoverMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryFailoverMonitoringValidator validator = new();
    private readonly RecoveryFailoverMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryFailoverMonitoringChanged>> ExecuteAsync(
        UpdateRecoveryFailoverMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryFailoverMonitoringChanged>.Invalid(issues);
        }

        RecoveryFailoverMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryFailoverMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryFailoverMonitoringChanged>.Invalid(
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

        RecoveryFailoverMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryFailoverMonitoringChanged>.Success(changed);
    }
}