namespace AtlasOps.Features.Edge.EdgePolicyGovernance;

using AtlasOps.Features;

public sealed class EdgePolicyGovernanceService(
    IAtlasOpsCapabilityRepository<EdgePolicyGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgePolicyGovernanceValidator validator = new();
    private readonly EdgePolicyGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgePolicyGovernanceChanged>> ExecuteAsync(
        UpdateEdgePolicyGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgePolicyGovernanceChanged>.Invalid(issues);
        }

        EdgePolicyGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgePolicyGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgePolicyGovernanceChanged>.Invalid(
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

        EdgePolicyGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgePolicyGovernanceChanged>.Success(changed);
    }
}