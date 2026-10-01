namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveGovernance;

using AtlasOps.Features;

public sealed class RecoveryObjectiveGovernanceService(
    IAtlasOpsCapabilityRepository<RecoveryObjectiveGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryObjectiveGovernanceValidator validator = new();
    private readonly RecoveryObjectiveGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryObjectiveGovernanceChanged>> ExecuteAsync(
        UpdateRecoveryObjectiveGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryObjectiveGovernanceChanged>.Invalid(issues);
        }

        RecoveryObjectiveGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryObjectiveGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryObjectiveGovernanceChanged>.Invalid(
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

        RecoveryObjectiveGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryObjectiveGovernanceChanged>.Success(changed);
    }
}