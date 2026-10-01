namespace AtlasOps.Features.Cloud.CloudDatabaseRecovery;

using AtlasOps.Features;

public sealed class CloudDatabaseRecoveryService(
    IAtlasOpsCapabilityRepository<CloudDatabaseRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudDatabaseRecoveryValidator validator = new();
    private readonly CloudDatabaseRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudDatabaseRecoveryChanged>> ExecuteAsync(
        UpdateCloudDatabaseRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudDatabaseRecoveryChanged>.Invalid(issues);
        }

        CloudDatabaseRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudDatabaseRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudDatabaseRecoveryChanged>.Invalid(
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

        CloudDatabaseRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudDatabaseRecoveryChanged>.Success(changed);
    }
}