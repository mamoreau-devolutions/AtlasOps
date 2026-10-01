namespace AtlasOps.Features.Desktop.DesktopLicenseProvisioning;

using AtlasOps.Features;

public sealed class DesktopLicenseProvisioningService(
    IAtlasOpsCapabilityRepository<DesktopLicenseProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopLicenseProvisioningValidator validator = new();
    private readonly DesktopLicenseProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopLicenseProvisioningChanged>> ExecuteAsync(
        UpdateDesktopLicenseProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopLicenseProvisioningChanged>.Invalid(issues);
        }

        DesktopLicenseProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopLicenseProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopLicenseProvisioningChanged>.Invalid(
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

        DesktopLicenseProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopLicenseProvisioningChanged>.Success(changed);
    }
}