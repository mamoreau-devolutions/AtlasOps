namespace AtlasOps.Features.Cloud.AzureSubscriptionProvisioning;

using AtlasOps.Features;

public sealed class AzureSubscriptionProvisioningService(
    IAtlasOpsCapabilityRepository<AzureSubscriptionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly AzureSubscriptionProvisioningValidator validator = new();
    private readonly AzureSubscriptionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AzureSubscriptionProvisioningChanged>> ExecuteAsync(
        UpdateAzureSubscriptionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AzureSubscriptionProvisioningChanged>.Invalid(issues);
        }

        AzureSubscriptionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AzureSubscriptionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AzureSubscriptionProvisioningChanged>.Invalid(
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

        AzureSubscriptionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AzureSubscriptionProvisioningChanged>.Success(changed);
    }
}