namespace AtlasOps.Features.Storage.ObjectBucketRecovery;

using AtlasOps.Features;

public sealed class ObjectBucketRecoveryService(
    IAtlasOpsCapabilityRepository<ObjectBucketRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObjectBucketRecoveryValidator validator = new();
    private readonly ObjectBucketRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObjectBucketRecoveryChanged>> ExecuteAsync(
        UpdateObjectBucketRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObjectBucketRecoveryChanged>.Invalid(issues);
        }

        ObjectBucketRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObjectBucketRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObjectBucketRecoveryChanged>.Invalid(
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

        ObjectBucketRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObjectBucketRecoveryChanged>.Success(changed);
    }
}