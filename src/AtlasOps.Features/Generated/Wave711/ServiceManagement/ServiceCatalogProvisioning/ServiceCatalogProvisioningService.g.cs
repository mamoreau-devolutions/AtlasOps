namespace AtlasOps.Features.ServiceManagement.ServiceCatalogProvisioning;

using AtlasOps.Features;

public sealed class ServiceCatalogProvisioningService(
    IAtlasOpsCapabilityRepository<ServiceCatalogProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceCatalogProvisioningValidator validator = new();
    private readonly ServiceCatalogProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceCatalogProvisioningChanged>> ExecuteAsync(
        UpdateServiceCatalogProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceCatalogProvisioningChanged>.Invalid(issues);
        }

        ServiceCatalogProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceCatalogProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceCatalogProvisioningChanged>.Invalid(
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

        ServiceCatalogProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceCatalogProvisioningChanged>.Success(changed);
    }
}