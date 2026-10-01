namespace AtlasOps.Features.ServiceManagement.ServiceRequestProvisioning;

using AtlasOps.Features;

public sealed class ServiceRequestProvisioningService(
    IAtlasOpsCapabilityRepository<ServiceRequestProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceRequestProvisioningValidator validator = new();
    private readonly ServiceRequestProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceRequestProvisioningChanged>> ExecuteAsync(
        UpdateServiceRequestProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceRequestProvisioningChanged>.Invalid(issues);
        }

        ServiceRequestProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceRequestProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceRequestProvisioningChanged>.Invalid(
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

        ServiceRequestProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceRequestProvisioningChanged>.Success(changed);
    }
}