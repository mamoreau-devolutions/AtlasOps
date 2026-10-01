namespace AtlasOps.Features.Desktop.DesktopPolicyMonitoring;

using AtlasOps.Features;

public sealed class DesktopPolicyMonitoringService(
    IAtlasOpsCapabilityRepository<DesktopPolicyMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPolicyMonitoringValidator validator = new();
    private readonly DesktopPolicyMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPolicyMonitoringChanged>> ExecuteAsync(
        UpdateDesktopPolicyMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPolicyMonitoringChanged>.Invalid(issues);
        }

        DesktopPolicyMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPolicyMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPolicyMonitoringChanged>.Invalid(
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

        DesktopPolicyMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPolicyMonitoringChanged>.Success(changed);
    }
}