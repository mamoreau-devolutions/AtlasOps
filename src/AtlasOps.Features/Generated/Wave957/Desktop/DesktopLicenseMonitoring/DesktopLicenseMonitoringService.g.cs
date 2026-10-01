namespace AtlasOps.Features.Desktop.DesktopLicenseMonitoring;

using AtlasOps.Features;

public sealed class DesktopLicenseMonitoringService(
    IAtlasOpsCapabilityRepository<DesktopLicenseMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopLicenseMonitoringValidator validator = new();
    private readonly DesktopLicenseMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopLicenseMonitoringChanged>> ExecuteAsync(
        UpdateDesktopLicenseMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopLicenseMonitoringChanged>.Invalid(issues);
        }

        DesktopLicenseMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopLicenseMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopLicenseMonitoringChanged>.Invalid(
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

        DesktopLicenseMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopLicenseMonitoringChanged>.Success(changed);
    }
}