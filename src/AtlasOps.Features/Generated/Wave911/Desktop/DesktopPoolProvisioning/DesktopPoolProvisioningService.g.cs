namespace AtlasOps.Features.Desktop.DesktopPoolProvisioning;

using AtlasOps.Features;

public sealed class DesktopPoolProvisioningService(
    IAtlasOpsCapabilityRepository<DesktopPoolProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPoolProvisioningValidator validator = new();
    private readonly DesktopPoolProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPoolProvisioningChanged>> ExecuteAsync(
        UpdateDesktopPoolProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPoolProvisioningChanged>.Invalid(issues);
        }

        DesktopPoolProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPoolProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPoolProvisioningChanged>.Invalid(
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

        DesktopPoolProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPoolProvisioningChanged>.Success(changed);
    }
}