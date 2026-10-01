namespace AtlasOps.Features.Edge.EdgeIncidentProvisioning;

using AtlasOps.Features;

public sealed class EdgeIncidentProvisioningService(
    IAtlasOpsCapabilityRepository<EdgeIncidentProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeIncidentProvisioningValidator validator = new();
    private readonly EdgeIncidentProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeIncidentProvisioningChanged>> ExecuteAsync(
        UpdateEdgeIncidentProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeIncidentProvisioningChanged>.Invalid(issues);
        }

        EdgeIncidentProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeIncidentProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeIncidentProvisioningChanged>.Invalid(
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

        EdgeIncidentProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeIncidentProvisioningChanged>.Success(changed);
    }
}