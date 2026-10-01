namespace AtlasOps.Features.Cloud.CloudStorageOptimization;

using AtlasOps.Features;

public sealed class CloudStorageOptimizationService(
    IAtlasOpsCapabilityRepository<CloudStorageOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudStorageOptimizationValidator validator = new();
    private readonly CloudStorageOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudStorageOptimizationChanged>> ExecuteAsync(
        UpdateCloudStorageOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudStorageOptimizationChanged>.Invalid(issues);
        }

        CloudStorageOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudStorageOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudStorageOptimizationChanged>.Invalid(
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

        CloudStorageOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudStorageOptimizationChanged>.Success(changed);
    }
}