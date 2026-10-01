namespace AtlasOps.Features.Cloud.CloudDatabaseOptimization;

using AtlasOps.Features;

public sealed class CloudDatabaseOptimizationService(
    IAtlasOpsCapabilityRepository<CloudDatabaseOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudDatabaseOptimizationValidator validator = new();
    private readonly CloudDatabaseOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudDatabaseOptimizationChanged>> ExecuteAsync(
        UpdateCloudDatabaseOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudDatabaseOptimizationChanged>.Invalid(issues);
        }

        CloudDatabaseOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudDatabaseOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudDatabaseOptimizationChanged>.Invalid(
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

        CloudDatabaseOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudDatabaseOptimizationChanged>.Success(changed);
    }
}