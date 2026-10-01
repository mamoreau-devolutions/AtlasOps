namespace AtlasOps.Features.Cloud.CloudFunctionProvisioning;

using AtlasOps.Features;

public sealed class CloudFunctionProvisioningService(
    IAtlasOpsCapabilityRepository<CloudFunctionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudFunctionProvisioningValidator validator = new();
    private readonly CloudFunctionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudFunctionProvisioningChanged>> ExecuteAsync(
        UpdateCloudFunctionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudFunctionProvisioningChanged>.Invalid(issues);
        }

        CloudFunctionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudFunctionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudFunctionProvisioningChanged>.Invalid(
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

        CloudFunctionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudFunctionProvisioningChanged>.Success(changed);
    }
}