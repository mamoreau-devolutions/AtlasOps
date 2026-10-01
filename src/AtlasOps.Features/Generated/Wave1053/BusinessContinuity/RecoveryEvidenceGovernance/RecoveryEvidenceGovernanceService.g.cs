namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceGovernance;

using AtlasOps.Features;

public sealed class RecoveryEvidenceGovernanceService(
    IAtlasOpsCapabilityRepository<RecoveryEvidenceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryEvidenceGovernanceValidator validator = new();
    private readonly RecoveryEvidenceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryEvidenceGovernanceChanged>> ExecuteAsync(
        UpdateRecoveryEvidenceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryEvidenceGovernanceChanged>.Invalid(issues);
        }

        RecoveryEvidenceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryEvidenceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryEvidenceGovernanceChanged>.Invalid(
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

        RecoveryEvidenceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryEvidenceGovernanceChanged>.Success(changed);
    }
}