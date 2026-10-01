namespace AtlasOps.Features.Mobile.MobileUpdateProvisioning;

using AtlasOps.Features;

public sealed class MobileUpdateProvisioningService(
    IAtlasOpsCapabilityRepository<MobileUpdateProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileUpdateProvisioningValidator validator = new();
    private readonly MobileUpdateProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileUpdateProvisioningChanged>> ExecuteAsync(
        UpdateMobileUpdateProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileUpdateProvisioningChanged>.Invalid(issues);
        }

        MobileUpdateProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileUpdateProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileUpdateProvisioningChanged>.Invalid(
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

        MobileUpdateProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileUpdateProvisioningChanged>.Success(changed);
    }
}