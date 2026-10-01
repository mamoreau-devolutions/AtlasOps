namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowProvisioning;

using AtlasOps.Features;

public sealed class MaintenanceWindowProvisioningService(
    IAtlasOpsCapabilityRepository<MaintenanceWindowProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MaintenanceWindowProvisioningValidator validator = new();
    private readonly MaintenanceWindowProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MaintenanceWindowProvisioningChanged>> ExecuteAsync(
        UpdateMaintenanceWindowProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MaintenanceWindowProvisioningChanged>.Invalid(issues);
        }

        MaintenanceWindowProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MaintenanceWindowProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MaintenanceWindowProvisioningChanged>.Invalid(
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

        MaintenanceWindowProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MaintenanceWindowProvisioningChanged>.Success(changed);
    }
}