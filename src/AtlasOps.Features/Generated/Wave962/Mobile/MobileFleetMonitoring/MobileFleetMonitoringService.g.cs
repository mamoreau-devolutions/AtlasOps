namespace AtlasOps.Features.Mobile.MobileFleetMonitoring;

using AtlasOps.Features;

public sealed class MobileFleetMonitoringService(
    IAtlasOpsCapabilityRepository<MobileFleetMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileFleetMonitoringValidator validator = new();
    private readonly MobileFleetMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileFleetMonitoringChanged>> ExecuteAsync(
        UpdateMobileFleetMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileFleetMonitoringChanged>.Invalid(issues);
        }

        MobileFleetMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileFleetMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileFleetMonitoringChanged>.Invalid(
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

        MobileFleetMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileFleetMonitoringChanged>.Success(changed);
    }
}