namespace AtlasOps.Features.Delivery.ReleaseRollbackGovernance;

using AtlasOps.Features;

public sealed class ReleaseRollbackGovernanceService(
    IAtlasOpsCapabilityRepository<ReleaseRollbackGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseRollbackGovernanceValidator validator = new();
    private readonly ReleaseRollbackGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseRollbackGovernanceChanged>> ExecuteAsync(
        UpdateReleaseRollbackGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseRollbackGovernanceChanged>.Invalid(issues);
        }

        ReleaseRollbackGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseRollbackGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseRollbackGovernanceChanged>.Invalid(
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

        ReleaseRollbackGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseRollbackGovernanceChanged>.Success(changed);
    }
}