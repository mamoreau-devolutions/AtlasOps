namespace AtlasOps.Features.Edge.EdgeDeploymentGovernance;

using AtlasOps.Features;

public sealed class EdgeDeploymentGovernanceService(
    IAtlasOpsCapabilityRepository<EdgeDeploymentGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeploymentGovernanceValidator validator = new();
    private readonly EdgeDeploymentGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeploymentGovernanceChanged>> ExecuteAsync(
        UpdateEdgeDeploymentGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeploymentGovernanceChanged>.Invalid(issues);
        }

        EdgeDeploymentGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeploymentGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeploymentGovernanceChanged>.Invalid(
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

        EdgeDeploymentGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeploymentGovernanceChanged>.Success(changed);
    }
}