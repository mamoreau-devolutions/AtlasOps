namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveMonitoring;

using AtlasOps.Features;

public sealed class RecoveryObjectiveMonitoringService(
    IAtlasOpsCapabilityRepository<RecoveryObjectiveMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryObjectiveMonitoringValidator validator = new();
    private readonly RecoveryObjectiveMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryObjectiveMonitoringChanged>> ExecuteAsync(
        UpdateRecoveryObjectiveMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryObjectiveMonitoringChanged>.Invalid(issues);
        }

        RecoveryObjectiveMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryObjectiveMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryObjectiveMonitoringChanged>.Invalid(
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

        RecoveryObjectiveMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryObjectiveMonitoringChanged>.Success(changed);
    }
}