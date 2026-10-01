namespace AtlasOps.Features.Desktop.DesktopApplicationProvisioning;

using AtlasOps.Features;

public sealed class DesktopApplicationProvisioningService(
    IAtlasOpsCapabilityRepository<DesktopApplicationProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopApplicationProvisioningValidator validator = new();
    private readonly DesktopApplicationProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopApplicationProvisioningChanged>> ExecuteAsync(
        UpdateDesktopApplicationProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopApplicationProvisioningChanged>.Invalid(issues);
        }

        DesktopApplicationProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopApplicationProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopApplicationProvisioningChanged>.Invalid(
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

        DesktopApplicationProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopApplicationProvisioningChanged>.Success(changed);
    }
}