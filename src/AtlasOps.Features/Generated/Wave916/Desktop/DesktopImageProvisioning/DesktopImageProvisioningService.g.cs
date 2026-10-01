namespace AtlasOps.Features.Desktop.DesktopImageProvisioning;

using AtlasOps.Features;

public sealed class DesktopImageProvisioningService(
    IAtlasOpsCapabilityRepository<DesktopImageProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopImageProvisioningValidator validator = new();
    private readonly DesktopImageProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopImageProvisioningChanged>> ExecuteAsync(
        UpdateDesktopImageProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopImageProvisioningChanged>.Invalid(issues);
        }

        DesktopImageProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopImageProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopImageProvisioningChanged>.Invalid(
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

        DesktopImageProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopImageProvisioningChanged>.Success(changed);
    }
}