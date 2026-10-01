namespace AtlasOps.Features.Cloud.CloudStorageRecovery;

using AtlasOps.Features;

public sealed class CloudStorageRecoveryService(
    IAtlasOpsCapabilityRepository<CloudStorageRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudStorageRecoveryValidator validator = new();
    private readonly CloudStorageRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudStorageRecoveryChanged>> ExecuteAsync(
        UpdateCloudStorageRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudStorageRecoveryChanged>.Invalid(issues);
        }

        CloudStorageRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudStorageRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudStorageRecoveryChanged>.Invalid(
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

        CloudStorageRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudStorageRecoveryChanged>.Success(changed);
    }
}