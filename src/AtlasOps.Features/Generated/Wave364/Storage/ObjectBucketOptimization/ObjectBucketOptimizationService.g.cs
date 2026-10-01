namespace AtlasOps.Features.Storage.ObjectBucketOptimization;

using AtlasOps.Features;

public sealed class ObjectBucketOptimizationService(
    IAtlasOpsCapabilityRepository<ObjectBucketOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObjectBucketOptimizationValidator validator = new();
    private readonly ObjectBucketOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObjectBucketOptimizationChanged>> ExecuteAsync(
        UpdateObjectBucketOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObjectBucketOptimizationChanged>.Invalid(issues);
        }

        ObjectBucketOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObjectBucketOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObjectBucketOptimizationChanged>.Invalid(
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

        ObjectBucketOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObjectBucketOptimizationChanged>.Success(changed);
    }
}