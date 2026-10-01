namespace AtlasOps.Features.Desktop.DesktopSessionProvisioning;

using AtlasOps.Features;

public sealed class DesktopSessionProvisioningService(
    IAtlasOpsCapabilityRepository<DesktopSessionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopSessionProvisioningValidator validator = new();
    private readonly DesktopSessionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopSessionProvisioningChanged>> ExecuteAsync(
        UpdateDesktopSessionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopSessionProvisioningChanged>.Invalid(issues);
        }

        DesktopSessionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopSessionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopSessionProvisioningChanged>.Invalid(
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

        DesktopSessionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopSessionProvisioningChanged>.Success(changed);
    }
}