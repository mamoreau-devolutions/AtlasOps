namespace AtlasOps.Features.Cloud.CloudIdentityProvisioning;

using AtlasOps.Features;

public sealed class CloudIdentityProvisioningService(
    IAtlasOpsCapabilityRepository<CloudIdentityProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudIdentityProvisioningValidator validator = new();
    private readonly CloudIdentityProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudIdentityProvisioningChanged>> ExecuteAsync(
        UpdateCloudIdentityProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudIdentityProvisioningChanged>.Invalid(issues);
        }

        CloudIdentityProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudIdentityProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudIdentityProvisioningChanged>.Invalid(
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

        CloudIdentityProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudIdentityProvisioningChanged>.Success(changed);
    }
}