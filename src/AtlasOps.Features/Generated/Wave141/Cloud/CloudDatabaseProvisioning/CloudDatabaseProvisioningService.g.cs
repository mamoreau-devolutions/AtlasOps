namespace AtlasOps.Features.Cloud.CloudDatabaseProvisioning;

using AtlasOps.Features;

public sealed class CloudDatabaseProvisioningService(
    IAtlasOpsCapabilityRepository<CloudDatabaseProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudDatabaseProvisioningValidator validator = new();
    private readonly CloudDatabaseProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudDatabaseProvisioningChanged>> ExecuteAsync(
        UpdateCloudDatabaseProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudDatabaseProvisioningChanged>.Invalid(issues);
        }

        CloudDatabaseProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudDatabaseProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudDatabaseProvisioningChanged>.Invalid(
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

        CloudDatabaseProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudDatabaseProvisioningChanged>.Success(changed);
    }
}