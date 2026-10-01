namespace AtlasOps.Features.Delivery.ReleaseGateGovernance;

using AtlasOps.Features;

public sealed class ReleaseGateGovernanceService(
    IAtlasOpsCapabilityRepository<ReleaseGateGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseGateGovernanceValidator validator = new();
    private readonly ReleaseGateGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseGateGovernanceChanged>> ExecuteAsync(
        UpdateReleaseGateGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseGateGovernanceChanged>.Invalid(issues);
        }

        ReleaseGateGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseGateGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseGateGovernanceChanged>.Invalid(
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

        ReleaseGateGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseGateGovernanceChanged>.Success(changed);
    }
}