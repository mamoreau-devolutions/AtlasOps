namespace AtlasOps.Features.Cloud.CloudRegionRecovery;

using AtlasOps.Features;

public sealed class CloudRegionRecoveryService(
    IAtlasOpsCapabilityRepository<CloudRegionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudRegionRecoveryValidator validator = new();
    private readonly CloudRegionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudRegionRecoveryChanged>> ExecuteAsync(
        UpdateCloudRegionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudRegionRecoveryChanged>.Invalid(issues);
        }

        CloudRegionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudRegionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudRegionRecoveryChanged>.Invalid(
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

        CloudRegionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudRegionRecoveryChanged>.Success(changed);
    }
}