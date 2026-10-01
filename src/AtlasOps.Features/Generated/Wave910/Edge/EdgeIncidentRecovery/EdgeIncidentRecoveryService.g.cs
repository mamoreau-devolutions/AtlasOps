namespace AtlasOps.Features.Edge.EdgeIncidentRecovery;

using AtlasOps.Features;

public sealed class EdgeIncidentRecoveryService(
    IAtlasOpsCapabilityRepository<EdgeIncidentRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeIncidentRecoveryValidator validator = new();
    private readonly EdgeIncidentRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeIncidentRecoveryChanged>> ExecuteAsync(
        UpdateEdgeIncidentRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeIncidentRecoveryChanged>.Invalid(issues);
        }

        EdgeIncidentRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeIncidentRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeIncidentRecoveryChanged>.Invalid(
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

        EdgeIncidentRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeIncidentRecoveryChanged>.Success(changed);
    }
}