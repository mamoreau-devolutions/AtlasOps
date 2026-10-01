namespace AtlasOps.Features.Compute.VirtualMachineGovernance;

using AtlasOps.Features;

public sealed class VirtualMachineGovernanceService(
    IAtlasOpsCapabilityRepository<VirtualMachineGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly VirtualMachineGovernanceValidator validator = new();
    private readonly VirtualMachineGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<VirtualMachineGovernanceChanged>> ExecuteAsync(
        UpdateVirtualMachineGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<VirtualMachineGovernanceChanged>.Invalid(issues);
        }

        VirtualMachineGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new VirtualMachineGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<VirtualMachineGovernanceChanged>.Invalid(
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

        VirtualMachineGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<VirtualMachineGovernanceChanged>.Success(changed);
    }
}