namespace AtlasOps.Features.Desktop.DesktopUpdateProvisioning;

using AtlasOps.Features;

public sealed class DesktopUpdateProvisioningService(
    IAtlasOpsCapabilityRepository<DesktopUpdateProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopUpdateProvisioningValidator validator = new();
    private readonly DesktopUpdateProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopUpdateProvisioningChanged>> ExecuteAsync(
        UpdateDesktopUpdateProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopUpdateProvisioningChanged>.Invalid(issues);
        }

        DesktopUpdateProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopUpdateProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopUpdateProvisioningChanged>.Invalid(
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

        DesktopUpdateProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopUpdateProvisioningChanged>.Success(changed);
    }
}