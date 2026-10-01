namespace AtlasOps.Features.Network.NetworkProbeOptimization;

using AtlasOps.Features;

public sealed class NetworkProbeOptimizationService(
    IAtlasOpsCapabilityRepository<NetworkProbeOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkProbeOptimizationValidator validator = new();
    private readonly NetworkProbeOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkProbeOptimizationChanged>> ExecuteAsync(
        UpdateNetworkProbeOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkProbeOptimizationChanged>.Invalid(issues);
        }

        NetworkProbeOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkProbeOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkProbeOptimizationChanged>.Invalid(
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

        NetworkProbeOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkProbeOptimizationChanged>.Success(changed);
    }
}