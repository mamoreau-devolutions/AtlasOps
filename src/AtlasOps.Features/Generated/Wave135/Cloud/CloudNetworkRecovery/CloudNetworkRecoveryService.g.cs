namespace AtlasOps.Features.Cloud.CloudNetworkRecovery;

using AtlasOps.Features;

public sealed class CloudNetworkRecoveryService(
    IAtlasOpsCapabilityRepository<CloudNetworkRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudNetworkRecoveryValidator validator = new();
    private readonly CloudNetworkRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudNetworkRecoveryChanged>> ExecuteAsync(
        UpdateCloudNetworkRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudNetworkRecoveryChanged>.Invalid(issues);
        }

        CloudNetworkRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudNetworkRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudNetworkRecoveryChanged>.Invalid(
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

        CloudNetworkRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudNetworkRecoveryChanged>.Success(changed);
    }
}