namespace AtlasOps.Features.Mobile.MobileComplianceProvisioning;

using AtlasOps.Features;

public sealed class MobileComplianceProvisioningService(
    IAtlasOpsCapabilityRepository<MobileComplianceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileComplianceProvisioningValidator validator = new();
    private readonly MobileComplianceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileComplianceProvisioningChanged>> ExecuteAsync(
        UpdateMobileComplianceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileComplianceProvisioningChanged>.Invalid(issues);
        }

        MobileComplianceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileComplianceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileComplianceProvisioningChanged>.Invalid(
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

        MobileComplianceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileComplianceProvisioningChanged>.Success(changed);
    }
}