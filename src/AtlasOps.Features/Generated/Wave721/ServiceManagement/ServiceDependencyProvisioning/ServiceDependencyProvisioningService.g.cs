namespace AtlasOps.Features.ServiceManagement.ServiceDependencyProvisioning;

using AtlasOps.Features;

public sealed class ServiceDependencyProvisioningService(
    IAtlasOpsCapabilityRepository<ServiceDependencyProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceDependencyProvisioningValidator validator = new();
    private readonly ServiceDependencyProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceDependencyProvisioningChanged>> ExecuteAsync(
        UpdateServiceDependencyProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceDependencyProvisioningChanged>.Invalid(issues);
        }

        ServiceDependencyProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceDependencyProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceDependencyProvisioningChanged>.Invalid(
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

        ServiceDependencyProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceDependencyProvisioningChanged>.Success(changed);
    }
}