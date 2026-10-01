namespace AtlasOps.Features.Edge.EdgeNetworkRecovery;

using AtlasOps.Features;

public sealed class EdgeNetworkRecoveryService(
    IAtlasOpsCapabilityRepository<EdgeNetworkRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeNetworkRecoveryValidator validator = new();
    private readonly EdgeNetworkRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeNetworkRecoveryChanged>> ExecuteAsync(
        UpdateEdgeNetworkRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeNetworkRecoveryChanged>.Invalid(issues);
        }

        EdgeNetworkRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeNetworkRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeNetworkRecoveryChanged>.Invalid(
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

        EdgeNetworkRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeNetworkRecoveryChanged>.Success(changed);
    }
}