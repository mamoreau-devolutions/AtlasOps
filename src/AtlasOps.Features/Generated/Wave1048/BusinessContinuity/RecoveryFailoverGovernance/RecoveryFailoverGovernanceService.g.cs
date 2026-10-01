namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverGovernance;

using AtlasOps.Features;

public sealed class RecoveryFailoverGovernanceService(
    IAtlasOpsCapabilityRepository<RecoveryFailoverGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryFailoverGovernanceValidator validator = new();
    private readonly RecoveryFailoverGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryFailoverGovernanceChanged>> ExecuteAsync(
        UpdateRecoveryFailoverGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryFailoverGovernanceChanged>.Invalid(issues);
        }

        RecoveryFailoverGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryFailoverGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryFailoverGovernanceChanged>.Invalid(
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

        RecoveryFailoverGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryFailoverGovernanceChanged>.Success(changed);
    }
}