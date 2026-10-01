namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupGovernance;

using AtlasOps.Features;

public sealed class RecoveryBackupGovernanceService(
    IAtlasOpsCapabilityRepository<RecoveryBackupGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryBackupGovernanceValidator validator = new();
    private readonly RecoveryBackupGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryBackupGovernanceChanged>> ExecuteAsync(
        UpdateRecoveryBackupGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryBackupGovernanceChanged>.Invalid(issues);
        }

        RecoveryBackupGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryBackupGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryBackupGovernanceChanged>.Invalid(
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

        RecoveryBackupGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryBackupGovernanceChanged>.Success(changed);
    }
}