namespace AtlasOps.Features.FinOps.SavingsPlanGovernance;

using AtlasOps.Features;

public sealed class SavingsPlanGovernanceService(
    IAtlasOpsCapabilityRepository<SavingsPlanGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SavingsPlanGovernanceValidator validator = new();
    private readonly SavingsPlanGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SavingsPlanGovernanceChanged>> ExecuteAsync(
        UpdateSavingsPlanGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SavingsPlanGovernanceChanged>.Invalid(issues);
        }

        SavingsPlanGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SavingsPlanGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SavingsPlanGovernanceChanged>.Invalid(
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

        SavingsPlanGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SavingsPlanGovernanceChanged>.Success(changed);
    }
}