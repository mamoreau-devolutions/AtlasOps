namespace AtlasOps.Features.FinOps.CloudInvoiceProvisioning;

using AtlasOps.Features;

public sealed class CloudInvoiceProvisioningService(
    IAtlasOpsCapabilityRepository<CloudInvoiceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudInvoiceProvisioningValidator validator = new();
    private readonly CloudInvoiceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudInvoiceProvisioningChanged>> ExecuteAsync(
        UpdateCloudInvoiceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudInvoiceProvisioningChanged>.Invalid(issues);
        }

        CloudInvoiceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudInvoiceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudInvoiceProvisioningChanged>.Invalid(
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

        CloudInvoiceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudInvoiceProvisioningChanged>.Success(changed);
    }
}