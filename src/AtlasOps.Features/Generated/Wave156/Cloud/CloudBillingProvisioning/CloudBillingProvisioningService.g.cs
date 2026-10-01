namespace AtlasOps.Features.Cloud.CloudBillingProvisioning;

using AtlasOps.Features;

public sealed class CloudBillingProvisioningService(
    IAtlasOpsCapabilityRepository<CloudBillingProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudBillingProvisioningValidator validator = new();
    private readonly CloudBillingProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudBillingProvisioningChanged>> ExecuteAsync(
        UpdateCloudBillingProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudBillingProvisioningChanged>.Invalid(issues);
        }

        CloudBillingProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudBillingProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudBillingProvisioningChanged>.Invalid(
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

        CloudBillingProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudBillingProvisioningChanged>.Success(changed);
    }
}