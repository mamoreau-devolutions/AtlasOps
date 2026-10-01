namespace AtlasOps.Features.Mobile.MobileProfileMonitoring;

using AtlasOps.Features;

public sealed class MobileProfileMonitoringService(
    IAtlasOpsCapabilityRepository<MobileProfileMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileProfileMonitoringValidator validator = new();
    private readonly MobileProfileMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileProfileMonitoringChanged>> ExecuteAsync(
        UpdateMobileProfileMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileProfileMonitoringChanged>.Invalid(issues);
        }

        MobileProfileMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileProfileMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileProfileMonitoringChanged>.Invalid(
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

        MobileProfileMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileProfileMonitoringChanged>.Success(changed);
    }
}