namespace AtlasOps.Features.Desktop.DesktopImageMonitoring;

using AtlasOps.Features;

public sealed class DesktopImageMonitoringService(
    IAtlasOpsCapabilityRepository<DesktopImageMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopImageMonitoringValidator validator = new();
    private readonly DesktopImageMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopImageMonitoringChanged>> ExecuteAsync(
        UpdateDesktopImageMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopImageMonitoringChanged>.Invalid(issues);
        }

        DesktopImageMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopImageMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopImageMonitoringChanged>.Invalid(
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

        DesktopImageMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopImageMonitoringChanged>.Success(changed);
    }
}