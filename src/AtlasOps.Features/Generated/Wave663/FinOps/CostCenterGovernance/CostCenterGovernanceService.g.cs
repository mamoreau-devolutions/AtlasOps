namespace AtlasOps.Features.FinOps.CostCenterGovernance;

using AtlasOps.Features;

public sealed class CostCenterGovernanceService(
    IAtlasOpsCapabilityRepository<CostCenterGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostCenterGovernanceValidator validator = new();
    private readonly CostCenterGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostCenterGovernanceChanged>> ExecuteAsync(
        UpdateCostCenterGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostCenterGovernanceChanged>.Invalid(issues);
        }

        CostCenterGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostCenterGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostCenterGovernanceChanged>.Invalid(
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

        CostCenterGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostCenterGovernanceChanged>.Success(changed);
    }
}