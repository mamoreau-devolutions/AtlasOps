namespace AtlasOps.Features.FinOps.CostAnomalyProvisioning;

using AtlasOps.Features;

public sealed class CostAnomalyProvisioningService(
    IAtlasOpsCapabilityRepository<CostAnomalyProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostAnomalyProvisioningValidator validator = new();
    private readonly CostAnomalyProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostAnomalyProvisioningChanged>> ExecuteAsync(
        UpdateCostAnomalyProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostAnomalyProvisioningChanged>.Invalid(issues);
        }

        CostAnomalyProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostAnomalyProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostAnomalyProvisioningChanged>.Invalid(
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

        CostAnomalyProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostAnomalyProvisioningChanged>.Success(changed);
    }
}