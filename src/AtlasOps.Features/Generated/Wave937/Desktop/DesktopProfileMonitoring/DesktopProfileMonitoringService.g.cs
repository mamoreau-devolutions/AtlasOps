namespace AtlasOps.Features.Desktop.DesktopProfileMonitoring;

using AtlasOps.Features;

public sealed class DesktopProfileMonitoringService(
    IAtlasOpsCapabilityRepository<DesktopProfileMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopProfileMonitoringValidator validator = new();
    private readonly DesktopProfileMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopProfileMonitoringChanged>> ExecuteAsync(
        UpdateDesktopProfileMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopProfileMonitoringChanged>.Invalid(issues);
        }

        DesktopProfileMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopProfileMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopProfileMonitoringChanged>.Invalid(
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

        DesktopProfileMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopProfileMonitoringChanged>.Success(changed);
    }
}