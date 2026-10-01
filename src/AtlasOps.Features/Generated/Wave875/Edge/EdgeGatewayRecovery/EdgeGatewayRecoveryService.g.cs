namespace AtlasOps.Features.Edge.EdgeGatewayRecovery;

using AtlasOps.Features;

public sealed class EdgeGatewayRecoveryService(
    IAtlasOpsCapabilityRepository<EdgeGatewayRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeGatewayRecoveryValidator validator = new();
    private readonly EdgeGatewayRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeGatewayRecoveryChanged>> ExecuteAsync(
        UpdateEdgeGatewayRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeGatewayRecoveryChanged>.Invalid(issues);
        }

        EdgeGatewayRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeGatewayRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeGatewayRecoveryChanged>.Invalid(
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

        EdgeGatewayRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeGatewayRecoveryChanged>.Success(changed);
    }
}