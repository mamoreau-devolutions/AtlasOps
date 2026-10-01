namespace AtlasOps.Features.Desktop.DesktopApplicationMonitoring;

using AtlasOps.Features;

public sealed class DesktopApplicationMonitoringService(
    IAtlasOpsCapabilityRepository<DesktopApplicationMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopApplicationMonitoringValidator validator = new();
    private readonly DesktopApplicationMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopApplicationMonitoringChanged>> ExecuteAsync(
        UpdateDesktopApplicationMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopApplicationMonitoringChanged>.Invalid(issues);
        }

        DesktopApplicationMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopApplicationMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopApplicationMonitoringChanged>.Invalid(
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

        DesktopApplicationMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopApplicationMonitoringChanged>.Success(changed);
    }
}