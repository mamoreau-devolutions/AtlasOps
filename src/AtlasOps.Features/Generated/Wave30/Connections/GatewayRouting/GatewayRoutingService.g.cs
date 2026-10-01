namespace AtlasOps.Features.Connections.GatewayRouting;

using AtlasOps.Features;

public sealed class GatewayRoutingService(
    IAtlasOpsCapabilityRepository<GatewayRoutingItem> repository,
    TimeProvider timeProvider)
{
    private readonly GatewayRoutingValidator validator = new();
    private readonly GatewayRoutingPolicy policy = new();

    public async Task<AtlasOpsOperationResult<GatewayRoutingChanged>> ExecuteAsync(
        UpdateGatewayRoutingCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<GatewayRoutingChanged>.Invalid(issues);
        }

        GatewayRoutingItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new GatewayRoutingItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<GatewayRoutingChanged>.Invalid(
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

        GatewayRoutingChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<GatewayRoutingChanged>.Success(changed);
    }
}