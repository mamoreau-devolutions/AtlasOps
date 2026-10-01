namespace AtlasOps.Features.Desktop.DesktopProfileProvisioning;

using AtlasOps.Features;

public sealed class DesktopProfileProvisioningService(
    IAtlasOpsCapabilityRepository<DesktopProfileProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopProfileProvisioningValidator validator = new();
    private readonly DesktopProfileProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopProfileProvisioningChanged>> ExecuteAsync(
        UpdateDesktopProfileProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopProfileProvisioningChanged>.Invalid(issues);
        }

        DesktopProfileProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopProfileProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopProfileProvisioningChanged>.Invalid(
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

        DesktopProfileProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopProfileProvisioningChanged>.Success(changed);
    }
}