namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceMonitoring;

using AtlasOps.Features;

public sealed class RecoveryEvidenceMonitoringService(
    IAtlasOpsCapabilityRepository<RecoveryEvidenceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryEvidenceMonitoringValidator validator = new();
    private readonly RecoveryEvidenceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryEvidenceMonitoringChanged>> ExecuteAsync(
        UpdateRecoveryEvidenceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryEvidenceMonitoringChanged>.Invalid(issues);
        }

        RecoveryEvidenceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryEvidenceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryEvidenceMonitoringChanged>.Invalid(
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

        RecoveryEvidenceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryEvidenceMonitoringChanged>.Success(changed);
    }
}