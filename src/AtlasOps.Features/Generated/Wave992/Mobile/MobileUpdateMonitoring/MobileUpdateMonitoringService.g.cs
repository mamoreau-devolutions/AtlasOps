namespace AtlasOps.Features.Mobile.MobileUpdateMonitoring;

using AtlasOps.Features;

public sealed class MobileUpdateMonitoringService(
    IAtlasOpsCapabilityRepository<MobileUpdateMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileUpdateMonitoringValidator validator = new();
    private readonly MobileUpdateMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileUpdateMonitoringChanged>> ExecuteAsync(
        UpdateMobileUpdateMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileUpdateMonitoringChanged>.Invalid(issues);
        }

        MobileUpdateMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileUpdateMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileUpdateMonitoringChanged>.Invalid(
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

        MobileUpdateMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileUpdateMonitoringChanged>.Success(changed);
    }
}