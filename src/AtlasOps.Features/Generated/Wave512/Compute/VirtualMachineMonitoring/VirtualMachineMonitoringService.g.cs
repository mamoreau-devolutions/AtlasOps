namespace AtlasOps.Features.Compute.VirtualMachineMonitoring;

using AtlasOps.Features;

public sealed class VirtualMachineMonitoringService(
    IAtlasOpsCapabilityRepository<VirtualMachineMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly VirtualMachineMonitoringValidator validator = new();
    private readonly VirtualMachineMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<VirtualMachineMonitoringChanged>> ExecuteAsync(
        UpdateVirtualMachineMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<VirtualMachineMonitoringChanged>.Invalid(issues);
        }

        VirtualMachineMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new VirtualMachineMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<VirtualMachineMonitoringChanged>.Invalid(
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

        VirtualMachineMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<VirtualMachineMonitoringChanged>.Success(changed);
    }
}