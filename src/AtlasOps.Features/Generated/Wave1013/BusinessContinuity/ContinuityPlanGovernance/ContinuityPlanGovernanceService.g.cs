namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanGovernance;

using AtlasOps.Features;

public sealed class ContinuityPlanGovernanceService(
    IAtlasOpsCapabilityRepository<ContinuityPlanGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ContinuityPlanGovernanceValidator validator = new();
    private readonly ContinuityPlanGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ContinuityPlanGovernanceChanged>> ExecuteAsync(
        UpdateContinuityPlanGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ContinuityPlanGovernanceChanged>.Invalid(issues);
        }

        ContinuityPlanGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ContinuityPlanGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ContinuityPlanGovernanceChanged>.Invalid(
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

        ContinuityPlanGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ContinuityPlanGovernanceChanged>.Success(changed);
    }
}