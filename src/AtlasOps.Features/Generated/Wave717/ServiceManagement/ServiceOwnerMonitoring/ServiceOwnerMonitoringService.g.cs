namespace AtlasOps.Features.ServiceManagement.ServiceOwnerMonitoring;

using AtlasOps.Features;

public sealed class ServiceOwnerMonitoringService(
    IAtlasOpsCapabilityRepository<ServiceOwnerMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceOwnerMonitoringValidator validator = new();
    private readonly ServiceOwnerMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceOwnerMonitoringChanged>> ExecuteAsync(
        UpdateServiceOwnerMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceOwnerMonitoringChanged>.Invalid(issues);
        }

        ServiceOwnerMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceOwnerMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceOwnerMonitoringChanged>.Invalid(
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

        ServiceOwnerMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceOwnerMonitoringChanged>.Success(changed);
    }
}