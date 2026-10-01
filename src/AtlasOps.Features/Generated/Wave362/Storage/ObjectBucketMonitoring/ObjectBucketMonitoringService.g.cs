namespace AtlasOps.Features.Storage.ObjectBucketMonitoring;

using AtlasOps.Features;

public sealed class ObjectBucketMonitoringService(
    IAtlasOpsCapabilityRepository<ObjectBucketMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObjectBucketMonitoringValidator validator = new();
    private readonly ObjectBucketMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObjectBucketMonitoringChanged>> ExecuteAsync(
        UpdateObjectBucketMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObjectBucketMonitoringChanged>.Invalid(issues);
        }

        ObjectBucketMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObjectBucketMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObjectBucketMonitoringChanged>.Invalid(
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

        ObjectBucketMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObjectBucketMonitoringChanged>.Success(changed);
    }
}