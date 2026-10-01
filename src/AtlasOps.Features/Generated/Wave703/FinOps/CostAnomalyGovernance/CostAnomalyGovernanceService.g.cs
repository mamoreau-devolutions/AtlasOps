namespace AtlasOps.Features.FinOps.CostAnomalyGovernance;

using AtlasOps.Features;

public sealed class CostAnomalyGovernanceService(
    IAtlasOpsCapabilityRepository<CostAnomalyGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostAnomalyGovernanceValidator validator = new();
    private readonly CostAnomalyGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostAnomalyGovernanceChanged>> ExecuteAsync(
        UpdateCostAnomalyGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostAnomalyGovernanceChanged>.Invalid(issues);
        }

        CostAnomalyGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostAnomalyGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostAnomalyGovernanceChanged>.Invalid(
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

        CostAnomalyGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostAnomalyGovernanceChanged>.Success(changed);
    }
}