namespace AtlasOps.Features.Desktop.DesktopPoolMonitoring;

using AtlasOps.Features;

public sealed class DesktopPoolMonitoringService(
    IAtlasOpsCapabilityRepository<DesktopPoolMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPoolMonitoringValidator validator = new();
    private readonly DesktopPoolMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPoolMonitoringChanged>> ExecuteAsync(
        UpdateDesktopPoolMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPoolMonitoringChanged>.Invalid(issues);
        }

        DesktopPoolMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPoolMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPoolMonitoringChanged>.Invalid(
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

        DesktopPoolMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPoolMonitoringChanged>.Success(changed);
    }
}