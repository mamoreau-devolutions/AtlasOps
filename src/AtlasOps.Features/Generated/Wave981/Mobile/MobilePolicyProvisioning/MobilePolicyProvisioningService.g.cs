namespace AtlasOps.Features.Mobile.MobilePolicyProvisioning;

using AtlasOps.Features;

public sealed class MobilePolicyProvisioningService(
    IAtlasOpsCapabilityRepository<MobilePolicyProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobilePolicyProvisioningValidator validator = new();
    private readonly MobilePolicyProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobilePolicyProvisioningChanged>> ExecuteAsync(
        UpdateMobilePolicyProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobilePolicyProvisioningChanged>.Invalid(issues);
        }

        MobilePolicyProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobilePolicyProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobilePolicyProvisioningChanged>.Invalid(
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

        MobilePolicyProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobilePolicyProvisioningChanged>.Success(changed);
    }
}