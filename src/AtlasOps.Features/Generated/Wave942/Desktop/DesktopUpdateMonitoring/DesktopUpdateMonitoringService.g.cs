namespace AtlasOps.Features.Desktop.DesktopUpdateMonitoring;

using AtlasOps.Features;

public sealed class DesktopUpdateMonitoringService(
    IAtlasOpsCapabilityRepository<DesktopUpdateMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopUpdateMonitoringValidator validator = new();
    private readonly DesktopUpdateMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopUpdateMonitoringChanged>> ExecuteAsync(
        UpdateDesktopUpdateMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopUpdateMonitoringChanged>.Invalid(issues);
        }

        DesktopUpdateMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopUpdateMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopUpdateMonitoringChanged>.Invalid(
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

        DesktopUpdateMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopUpdateMonitoringChanged>.Success(changed);
    }
}