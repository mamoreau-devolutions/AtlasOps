namespace AtlasOps.Features.Edge.EdgeGatewayProvisioning;

using AtlasOps.Features;

public sealed class EdgeGatewayProvisioningService(
    IAtlasOpsCapabilityRepository<EdgeGatewayProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeGatewayProvisioningValidator validator = new();
    private readonly EdgeGatewayProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeGatewayProvisioningChanged>> ExecuteAsync(
        UpdateEdgeGatewayProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeGatewayProvisioningChanged>.Invalid(issues);
        }

        EdgeGatewayProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeGatewayProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeGatewayProvisioningChanged>.Invalid(
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

        EdgeGatewayProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeGatewayProvisioningChanged>.Success(changed);
    }
}