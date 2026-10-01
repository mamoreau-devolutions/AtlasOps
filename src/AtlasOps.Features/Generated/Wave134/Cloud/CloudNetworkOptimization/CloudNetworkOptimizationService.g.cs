namespace AtlasOps.Features.Cloud.CloudNetworkOptimization;

using AtlasOps.Features;

public sealed class CloudNetworkOptimizationService(
    IAtlasOpsCapabilityRepository<CloudNetworkOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudNetworkOptimizationValidator validator = new();
    private readonly CloudNetworkOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudNetworkOptimizationChanged>> ExecuteAsync(
        UpdateCloudNetworkOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudNetworkOptimizationChanged>.Invalid(issues);
        }

        CloudNetworkOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudNetworkOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudNetworkOptimizationChanged>.Invalid(
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

        CloudNetworkOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudNetworkOptimizationChanged>.Success(changed);
    }
}