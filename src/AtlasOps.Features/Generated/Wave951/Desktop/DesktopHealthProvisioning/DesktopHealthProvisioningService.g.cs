namespace AtlasOps.Features.Desktop.DesktopHealthProvisioning;

using AtlasOps.Features;

public sealed class DesktopHealthProvisioningService(
    IAtlasOpsCapabilityRepository<DesktopHealthProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopHealthProvisioningValidator validator = new();
    private readonly DesktopHealthProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopHealthProvisioningChanged>> ExecuteAsync(
        UpdateDesktopHealthProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopHealthProvisioningChanged>.Invalid(issues);
        }

        DesktopHealthProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopHealthProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopHealthProvisioningChanged>.Invalid(
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

        DesktopHealthProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopHealthProvisioningChanged>.Success(changed);
    }
}