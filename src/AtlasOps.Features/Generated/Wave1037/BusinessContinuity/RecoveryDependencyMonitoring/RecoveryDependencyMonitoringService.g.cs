namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyMonitoring;

using AtlasOps.Features;

public sealed class RecoveryDependencyMonitoringService(
    IAtlasOpsCapabilityRepository<RecoveryDependencyMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryDependencyMonitoringValidator validator = new();
    private readonly RecoveryDependencyMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryDependencyMonitoringChanged>> ExecuteAsync(
        UpdateRecoveryDependencyMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryDependencyMonitoringChanged>.Invalid(issues);
        }

        RecoveryDependencyMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryDependencyMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryDependencyMonitoringChanged>.Invalid(
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

        RecoveryDependencyMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryDependencyMonitoringChanged>.Success(changed);
    }
}