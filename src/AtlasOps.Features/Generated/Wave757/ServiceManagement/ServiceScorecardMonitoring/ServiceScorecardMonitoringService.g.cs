namespace AtlasOps.Features.ServiceManagement.ServiceScorecardMonitoring;

using AtlasOps.Features;

public sealed class ServiceScorecardMonitoringService(
    IAtlasOpsCapabilityRepository<ServiceScorecardMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceScorecardMonitoringValidator validator = new();
    private readonly ServiceScorecardMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceScorecardMonitoringChanged>> ExecuteAsync(
        UpdateServiceScorecardMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceScorecardMonitoringChanged>.Invalid(issues);
        }

        ServiceScorecardMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceScorecardMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceScorecardMonitoringChanged>.Invalid(
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

        ServiceScorecardMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceScorecardMonitoringChanged>.Success(changed);
    }
}