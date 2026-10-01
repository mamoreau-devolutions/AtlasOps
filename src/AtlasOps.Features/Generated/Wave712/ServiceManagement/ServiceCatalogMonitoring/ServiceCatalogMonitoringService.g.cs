namespace AtlasOps.Features.ServiceManagement.ServiceCatalogMonitoring;

using AtlasOps.Features;

public sealed class ServiceCatalogMonitoringService(
    IAtlasOpsCapabilityRepository<ServiceCatalogMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceCatalogMonitoringValidator validator = new();
    private readonly ServiceCatalogMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceCatalogMonitoringChanged>> ExecuteAsync(
        UpdateServiceCatalogMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceCatalogMonitoringChanged>.Invalid(issues);
        }

        ServiceCatalogMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceCatalogMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceCatalogMonitoringChanged>.Invalid(
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

        ServiceCatalogMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceCatalogMonitoringChanged>.Success(changed);
    }
}