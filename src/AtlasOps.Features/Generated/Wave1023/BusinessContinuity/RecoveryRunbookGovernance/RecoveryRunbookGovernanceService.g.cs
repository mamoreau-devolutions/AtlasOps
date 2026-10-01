namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookGovernance;

using AtlasOps.Features;

public sealed class RecoveryRunbookGovernanceService(
    IAtlasOpsCapabilityRepository<RecoveryRunbookGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryRunbookGovernanceValidator validator = new();
    private readonly RecoveryRunbookGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryRunbookGovernanceChanged>> ExecuteAsync(
        UpdateRecoveryRunbookGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryRunbookGovernanceChanged>.Invalid(issues);
        }

        RecoveryRunbookGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryRunbookGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryRunbookGovernanceChanged>.Invalid(
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

        RecoveryRunbookGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryRunbookGovernanceChanged>.Success(changed);
    }
}