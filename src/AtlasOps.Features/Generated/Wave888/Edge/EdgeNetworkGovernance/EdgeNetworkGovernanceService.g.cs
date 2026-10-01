namespace AtlasOps.Features.Edge.EdgeNetworkGovernance;

using AtlasOps.Features;

public sealed class EdgeNetworkGovernanceService(
    IAtlasOpsCapabilityRepository<EdgeNetworkGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeNetworkGovernanceValidator validator = new();
    private readonly EdgeNetworkGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeNetworkGovernanceChanged>> ExecuteAsync(
        UpdateEdgeNetworkGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeNetworkGovernanceChanged>.Invalid(issues);
        }

        EdgeNetworkGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeNetworkGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeNetworkGovernanceChanged>.Invalid(
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

        EdgeNetworkGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeNetworkGovernanceChanged>.Success(changed);
    }
}