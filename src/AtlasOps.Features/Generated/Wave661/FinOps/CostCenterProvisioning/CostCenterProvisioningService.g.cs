namespace AtlasOps.Features.FinOps.CostCenterProvisioning;

using AtlasOps.Features;

public sealed class CostCenterProvisioningService(
    IAtlasOpsCapabilityRepository<CostCenterProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostCenterProvisioningValidator validator = new();
    private readonly CostCenterProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostCenterProvisioningChanged>> ExecuteAsync(
        UpdateCostCenterProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostCenterProvisioningChanged>.Invalid(issues);
        }

        CostCenterProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostCenterProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostCenterProvisioningChanged>.Invalid(
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

        CostCenterProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostCenterProvisioningChanged>.Success(changed);
    }
}