namespace AtlasOps.Features.Edge.EdgeDeviceGovernance;

using AtlasOps.Features;

public sealed class EdgeDeviceGovernanceService(
    IAtlasOpsCapabilityRepository<EdgeDeviceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeviceGovernanceValidator validator = new();
    private readonly EdgeDeviceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeviceGovernanceChanged>> ExecuteAsync(
        UpdateEdgeDeviceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeviceGovernanceChanged>.Invalid(issues);
        }

        EdgeDeviceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeviceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeviceGovernanceChanged>.Invalid(
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

        EdgeDeviceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeviceGovernanceChanged>.Success(changed);
    }
}