namespace AtlasOps.Features.ServiceManagement.ServiceDependencyMonitoring;

using AtlasOps.Features;

public sealed class ServiceDependencyMonitoringService(
    IAtlasOpsCapabilityRepository<ServiceDependencyMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceDependencyMonitoringValidator validator = new();
    private readonly ServiceDependencyMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceDependencyMonitoringChanged>> ExecuteAsync(
        UpdateServiceDependencyMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceDependencyMonitoringChanged>.Invalid(issues);
        }

        ServiceDependencyMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceDependencyMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceDependencyMonitoringChanged>.Invalid(
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

        ServiceDependencyMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceDependencyMonitoringChanged>.Success(changed);
    }
}