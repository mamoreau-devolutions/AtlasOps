namespace AtlasOps.Features.BusinessContinuity.RecoverySiteGovernance;

using AtlasOps.Features;

public sealed class RecoverySiteGovernanceService(
    IAtlasOpsCapabilityRepository<RecoverySiteGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoverySiteGovernanceValidator validator = new();
    private readonly RecoverySiteGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoverySiteGovernanceChanged>> ExecuteAsync(
        UpdateRecoverySiteGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoverySiteGovernanceChanged>.Invalid(issues);
        }

        RecoverySiteGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoverySiteGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoverySiteGovernanceChanged>.Invalid(
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

        RecoverySiteGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoverySiteGovernanceChanged>.Success(changed);
    }
}