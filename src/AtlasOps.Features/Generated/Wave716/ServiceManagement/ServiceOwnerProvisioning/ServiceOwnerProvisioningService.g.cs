namespace AtlasOps.Features.ServiceManagement.ServiceOwnerProvisioning;

using AtlasOps.Features;

public sealed class ServiceOwnerProvisioningService(
    IAtlasOpsCapabilityRepository<ServiceOwnerProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceOwnerProvisioningValidator validator = new();
    private readonly ServiceOwnerProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceOwnerProvisioningChanged>> ExecuteAsync(
        UpdateServiceOwnerProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceOwnerProvisioningChanged>.Invalid(issues);
        }

        ServiceOwnerProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceOwnerProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceOwnerProvisioningChanged>.Invalid(
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

        ServiceOwnerProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceOwnerProvisioningChanged>.Success(changed);
    }
}