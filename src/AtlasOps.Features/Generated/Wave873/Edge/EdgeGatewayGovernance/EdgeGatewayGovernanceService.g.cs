namespace AtlasOps.Features.Edge.EdgeGatewayGovernance;

using AtlasOps.Features;

public sealed class EdgeGatewayGovernanceService(
    IAtlasOpsCapabilityRepository<EdgeGatewayGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeGatewayGovernanceValidator validator = new();
    private readonly EdgeGatewayGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeGatewayGovernanceChanged>> ExecuteAsync(
        UpdateEdgeGatewayGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeGatewayGovernanceChanged>.Invalid(issues);
        }

        EdgeGatewayGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeGatewayGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeGatewayGovernanceChanged>.Invalid(
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

        EdgeGatewayGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeGatewayGovernanceChanged>.Success(changed);
    }
}