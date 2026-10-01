namespace AtlasOps.Features.Mobile.MobileComplianceMonitoring;

using AtlasOps.Features;

public sealed class MobileComplianceMonitoringService(
    IAtlasOpsCapabilityRepository<MobileComplianceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileComplianceMonitoringValidator validator = new();
    private readonly MobileComplianceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileComplianceMonitoringChanged>> ExecuteAsync(
        UpdateMobileComplianceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileComplianceMonitoringChanged>.Invalid(issues);
        }

        MobileComplianceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileComplianceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileComplianceMonitoringChanged>.Invalid(
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

        MobileComplianceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileComplianceMonitoringChanged>.Success(changed);
    }
}