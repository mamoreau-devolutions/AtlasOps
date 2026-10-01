namespace AtlasOps.Features.Edge.EdgeNetworkProvisioning;

using AtlasOps.Features;

public sealed class EdgeNetworkProvisioningService(
    IAtlasOpsCapabilityRepository<EdgeNetworkProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeNetworkProvisioningValidator validator = new();
    private readonly EdgeNetworkProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeNetworkProvisioningChanged>> ExecuteAsync(
        UpdateEdgeNetworkProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeNetworkProvisioningChanged>.Invalid(issues);
        }

        EdgeNetworkProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeNetworkProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeNetworkProvisioningChanged>.Invalid(
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

        EdgeNetworkProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeNetworkProvisioningChanged>.Success(changed);
    }
}