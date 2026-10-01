namespace AtlasOps.Features.Desktop.DesktopPeripheralMonitoring;

using AtlasOps.Features;

public sealed class DesktopPeripheralMonitoringService(
    IAtlasOpsCapabilityRepository<DesktopPeripheralMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPeripheralMonitoringValidator validator = new();
    private readonly DesktopPeripheralMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPeripheralMonitoringChanged>> ExecuteAsync(
        UpdateDesktopPeripheralMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPeripheralMonitoringChanged>.Invalid(issues);
        }

        DesktopPeripheralMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPeripheralMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPeripheralMonitoringChanged>.Invalid(
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

        DesktopPeripheralMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPeripheralMonitoringChanged>.Success(changed);
    }
}