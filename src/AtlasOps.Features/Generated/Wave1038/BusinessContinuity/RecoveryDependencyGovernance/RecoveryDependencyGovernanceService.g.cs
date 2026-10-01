namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyGovernance;

using AtlasOps.Features;

public sealed class RecoveryDependencyGovernanceService(
    IAtlasOpsCapabilityRepository<RecoveryDependencyGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryDependencyGovernanceValidator validator = new();
    private readonly RecoveryDependencyGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryDependencyGovernanceChanged>> ExecuteAsync(
        UpdateRecoveryDependencyGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryDependencyGovernanceChanged>.Invalid(issues);
        }

        RecoveryDependencyGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryDependencyGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryDependencyGovernanceChanged>.Invalid(
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

        RecoveryDependencyGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryDependencyGovernanceChanged>.Success(changed);
    }
}