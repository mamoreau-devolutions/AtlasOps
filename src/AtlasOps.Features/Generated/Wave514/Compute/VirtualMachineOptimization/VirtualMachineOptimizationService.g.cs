namespace AtlasOps.Features.Compute.VirtualMachineOptimization;

using AtlasOps.Features;

public sealed class VirtualMachineOptimizationService(
    IAtlasOpsCapabilityRepository<VirtualMachineOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly VirtualMachineOptimizationValidator validator = new();
    private readonly VirtualMachineOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<VirtualMachineOptimizationChanged>> ExecuteAsync(
        UpdateVirtualMachineOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<VirtualMachineOptimizationChanged>.Invalid(issues);
        }

        VirtualMachineOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new VirtualMachineOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<VirtualMachineOptimizationChanged>.Invalid(
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

        VirtualMachineOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<VirtualMachineOptimizationChanged>.Success(changed);
    }
}