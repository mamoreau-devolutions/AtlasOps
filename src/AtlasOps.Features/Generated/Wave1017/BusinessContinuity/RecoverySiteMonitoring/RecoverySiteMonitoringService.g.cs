namespace AtlasOps.Features.BusinessContinuity.RecoverySiteMonitoring;

using AtlasOps.Features;

public sealed class RecoverySiteMonitoringService(
    IAtlasOpsCapabilityRepository<RecoverySiteMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoverySiteMonitoringValidator validator = new();
    private readonly RecoverySiteMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoverySiteMonitoringChanged>> ExecuteAsync(
        UpdateRecoverySiteMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoverySiteMonitoringChanged>.Invalid(issues);
        }

        RecoverySiteMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoverySiteMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoverySiteMonitoringChanged>.Invalid(
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

        RecoverySiteMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoverySiteMonitoringChanged>.Success(changed);
    }
}