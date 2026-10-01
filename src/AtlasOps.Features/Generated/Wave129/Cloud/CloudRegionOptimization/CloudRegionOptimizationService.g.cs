namespace AtlasOps.Features.Cloud.CloudRegionOptimization;

using AtlasOps.Features;

public sealed class CloudRegionOptimizationService(
    IAtlasOpsCapabilityRepository<CloudRegionOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudRegionOptimizationValidator validator = new();
    private readonly CloudRegionOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudRegionOptimizationChanged>> ExecuteAsync(
        UpdateCloudRegionOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudRegionOptimizationChanged>.Invalid(issues);
        }

        CloudRegionOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudRegionOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudRegionOptimizationChanged>.Invalid(
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

        CloudRegionOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudRegionOptimizationChanged>.Success(changed);
    }
}