namespace AtlasOps.Features.Mobile.MobileSupportProvisioning;

using AtlasOps.Features;

public sealed class MobileSupportProvisioningService(
    IAtlasOpsCapabilityRepository<MobileSupportProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileSupportProvisioningValidator validator = new();
    private readonly MobileSupportProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileSupportProvisioningChanged>> ExecuteAsync(
        UpdateMobileSupportProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileSupportProvisioningChanged>.Invalid(issues);
        }

        MobileSupportProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileSupportProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileSupportProvisioningChanged>.Invalid(
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

        MobileSupportProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileSupportProvisioningChanged>.Success(changed);
    }
}