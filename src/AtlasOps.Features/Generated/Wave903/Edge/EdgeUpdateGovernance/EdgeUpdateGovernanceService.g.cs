namespace AtlasOps.Features.Edge.EdgeUpdateGovernance;

using AtlasOps.Features;

public sealed class EdgeUpdateGovernanceService(
    IAtlasOpsCapabilityRepository<EdgeUpdateGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeUpdateGovernanceValidator validator = new();
    private readonly EdgeUpdateGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeUpdateGovernanceChanged>> ExecuteAsync(
        UpdateEdgeUpdateGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeUpdateGovernanceChanged>.Invalid(issues);
        }

        EdgeUpdateGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeUpdateGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeUpdateGovernanceChanged>.Invalid(
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

        EdgeUpdateGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeUpdateGovernanceChanged>.Success(changed);
    }
}