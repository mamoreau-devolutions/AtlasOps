namespace AtlasOps.Features.FinOps.CostAllocationProvisioning;

using AtlasOps.Features;

public sealed class CostAllocationProvisioningService(
    IAtlasOpsCapabilityRepository<CostAllocationProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostAllocationProvisioningValidator validator = new();
    private readonly CostAllocationProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostAllocationProvisioningChanged>> ExecuteAsync(
        UpdateCostAllocationProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostAllocationProvisioningChanged>.Invalid(issues);
        }

        CostAllocationProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostAllocationProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostAllocationProvisioningChanged>.Invalid(
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

        CostAllocationProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostAllocationProvisioningChanged>.Success(changed);
    }
}