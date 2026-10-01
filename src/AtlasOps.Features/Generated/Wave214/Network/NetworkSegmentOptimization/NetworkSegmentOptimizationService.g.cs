namespace AtlasOps.Features.Network.NetworkSegmentOptimization;

using AtlasOps.Features;

public sealed class NetworkSegmentOptimizationService(
    IAtlasOpsCapabilityRepository<NetworkSegmentOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkSegmentOptimizationValidator validator = new();
    private readonly NetworkSegmentOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkSegmentOptimizationChanged>> ExecuteAsync(
        UpdateNetworkSegmentOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkSegmentOptimizationChanged>.Invalid(issues);
        }

        NetworkSegmentOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkSegmentOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkSegmentOptimizationChanged>.Invalid(
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

        NetworkSegmentOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkSegmentOptimizationChanged>.Success(changed);
    }
}