namespace AtlasOps.Features.Compute.VirtualMachineProvisioning;

using AtlasOps.Features;

public sealed class VirtualMachineProvisioningService(
    IAtlasOpsCapabilityRepository<VirtualMachineProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly VirtualMachineProvisioningValidator validator = new();
    private readonly VirtualMachineProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<VirtualMachineProvisioningChanged>> ExecuteAsync(
        UpdateVirtualMachineProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<VirtualMachineProvisioningChanged>.Invalid(issues);
        }

        VirtualMachineProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new VirtualMachineProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<VirtualMachineProvisioningChanged>.Invalid(
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

        VirtualMachineProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<VirtualMachineProvisioningChanged>.Success(changed);
    }
}