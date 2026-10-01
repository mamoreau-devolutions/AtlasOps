namespace AtlasOps.Features.Mobile.MobileProfileProvisioning;

using AtlasOps.Features;

public sealed class MobileProfileProvisioningService(
    IAtlasOpsCapabilityRepository<MobileProfileProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileProfileProvisioningValidator validator = new();
    private readonly MobileProfileProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileProfileProvisioningChanged>> ExecuteAsync(
        UpdateMobileProfileProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileProfileProvisioningChanged>.Invalid(issues);
        }

        MobileProfileProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileProfileProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileProfileProvisioningChanged>.Invalid(
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

        MobileProfileProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileProfileProvisioningChanged>.Success(changed);
    }
}