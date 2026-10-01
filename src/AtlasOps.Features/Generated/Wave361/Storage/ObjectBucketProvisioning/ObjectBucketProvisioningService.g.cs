namespace AtlasOps.Features.Storage.ObjectBucketProvisioning;

using AtlasOps.Features;

public sealed class ObjectBucketProvisioningService(
    IAtlasOpsCapabilityRepository<ObjectBucketProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObjectBucketProvisioningValidator validator = new();
    private readonly ObjectBucketProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObjectBucketProvisioningChanged>> ExecuteAsync(
        UpdateObjectBucketProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObjectBucketProvisioningChanged>.Invalid(issues);
        }

        ObjectBucketProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObjectBucketProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObjectBucketProvisioningChanged>.Invalid(
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

        ObjectBucketProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObjectBucketProvisioningChanged>.Success(changed);
    }
}