namespace AtlasOps.Features.Compute.VirtualMachineRecovery;

using AtlasOps.Features;

public sealed class VirtualMachineRecoveryService(
    IAtlasOpsCapabilityRepository<VirtualMachineRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly VirtualMachineRecoveryValidator validator = new();
    private readonly VirtualMachineRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<VirtualMachineRecoveryChanged>> ExecuteAsync(
        UpdateVirtualMachineRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<VirtualMachineRecoveryChanged>.Invalid(issues);
        }

        VirtualMachineRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new VirtualMachineRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<VirtualMachineRecoveryChanged>.Invalid(
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

        VirtualMachineRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<VirtualMachineRecoveryChanged>.Success(changed);
    }
}