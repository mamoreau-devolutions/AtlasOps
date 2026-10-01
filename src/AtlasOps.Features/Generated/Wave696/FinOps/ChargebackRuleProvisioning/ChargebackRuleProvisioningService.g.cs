namespace AtlasOps.Features.FinOps.ChargebackRuleProvisioning;

using AtlasOps.Features;

public sealed class ChargebackRuleProvisioningService(
    IAtlasOpsCapabilityRepository<ChargebackRuleProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChargebackRuleProvisioningValidator validator = new();
    private readonly ChargebackRuleProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChargebackRuleProvisioningChanged>> ExecuteAsync(
        UpdateChargebackRuleProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChargebackRuleProvisioningChanged>.Invalid(issues);
        }

        ChargebackRuleProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChargebackRuleProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChargebackRuleProvisioningChanged>.Invalid(
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

        ChargebackRuleProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChargebackRuleProvisioningChanged>.Success(changed);
    }
}