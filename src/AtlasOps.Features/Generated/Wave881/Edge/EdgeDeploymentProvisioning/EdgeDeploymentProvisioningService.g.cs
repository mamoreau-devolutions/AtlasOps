namespace AtlasOps.Features.Edge.EdgeDeploymentProvisioning;

using AtlasOps.Features;

public sealed class EdgeDeploymentProvisioningService(
    IAtlasOpsCapabilityRepository<EdgeDeploymentProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeploymentProvisioningValidator validator = new();
    private readonly EdgeDeploymentProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeploymentProvisioningChanged>> ExecuteAsync(
        UpdateEdgeDeploymentProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeploymentProvisioningChanged>.Invalid(issues);
        }

        EdgeDeploymentProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeploymentProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeploymentProvisioningChanged>.Invalid(
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

        EdgeDeploymentProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeploymentProvisioningChanged>.Success(changed);
    }
}