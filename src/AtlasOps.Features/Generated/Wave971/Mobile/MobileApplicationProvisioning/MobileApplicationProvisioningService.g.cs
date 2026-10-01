namespace AtlasOps.Features.Mobile.MobileApplicationProvisioning;

using AtlasOps.Features;

public sealed class MobileApplicationProvisioningService(
    IAtlasOpsCapabilityRepository<MobileApplicationProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileApplicationProvisioningValidator validator = new();
    private readonly MobileApplicationProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileApplicationProvisioningChanged>> ExecuteAsync(
        UpdateMobileApplicationProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileApplicationProvisioningChanged>.Invalid(issues);
        }

        MobileApplicationProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileApplicationProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileApplicationProvisioningChanged>.Invalid(
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

        MobileApplicationProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileApplicationProvisioningChanged>.Success(changed);
    }
}