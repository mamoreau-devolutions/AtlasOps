namespace AtlasOps.Features.Mobile.MobileFleetProvisioning;

using AtlasOps.Features;

public sealed class MobileFleetProvisioningService(
    IAtlasOpsCapabilityRepository<MobileFleetProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileFleetProvisioningValidator validator = new();
    private readonly MobileFleetProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileFleetProvisioningChanged>> ExecuteAsync(
        UpdateMobileFleetProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileFleetProvisioningChanged>.Invalid(issues);
        }

        MobileFleetProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileFleetProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileFleetProvisioningChanged>.Invalid(
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

        MobileFleetProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileFleetProvisioningChanged>.Success(changed);
    }
}