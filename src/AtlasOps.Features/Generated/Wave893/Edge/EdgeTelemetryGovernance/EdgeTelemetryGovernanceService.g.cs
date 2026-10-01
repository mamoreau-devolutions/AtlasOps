namespace AtlasOps.Features.Edge.EdgeTelemetryGovernance;

using AtlasOps.Features;

public sealed class EdgeTelemetryGovernanceService(
    IAtlasOpsCapabilityRepository<EdgeTelemetryGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeTelemetryGovernanceValidator validator = new();
    private readonly EdgeTelemetryGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeTelemetryGovernanceChanged>> ExecuteAsync(
        UpdateEdgeTelemetryGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeTelemetryGovernanceChanged>.Invalid(issues);
        }

        EdgeTelemetryGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeTelemetryGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeTelemetryGovernanceChanged>.Invalid(
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

        EdgeTelemetryGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeTelemetryGovernanceChanged>.Success(changed);
    }
}