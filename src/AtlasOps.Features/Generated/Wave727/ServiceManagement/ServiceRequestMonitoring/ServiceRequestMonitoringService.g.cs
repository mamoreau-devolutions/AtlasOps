namespace AtlasOps.Features.ServiceManagement.ServiceRequestMonitoring;

using AtlasOps.Features;

public sealed class ServiceRequestMonitoringService(
    IAtlasOpsCapabilityRepository<ServiceRequestMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceRequestMonitoringValidator validator = new();
    private readonly ServiceRequestMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceRequestMonitoringChanged>> ExecuteAsync(
        UpdateServiceRequestMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceRequestMonitoringChanged>.Invalid(issues);
        }

        ServiceRequestMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceRequestMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceRequestMonitoringChanged>.Invalid(
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

        ServiceRequestMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceRequestMonitoringChanged>.Success(changed);
    }
}