namespace AtlasOps.Features.FinOps.ChargebackRuleGovernance;

using AtlasOps.Features;

public sealed class ChargebackRuleGovernanceService(
    IAtlasOpsCapabilityRepository<ChargebackRuleGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChargebackRuleGovernanceValidator validator = new();
    private readonly ChargebackRuleGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChargebackRuleGovernanceChanged>> ExecuteAsync(
        UpdateChargebackRuleGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChargebackRuleGovernanceChanged>.Invalid(issues);
        }

        ChargebackRuleGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChargebackRuleGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChargebackRuleGovernanceChanged>.Invalid(
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

        ChargebackRuleGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChargebackRuleGovernanceChanged>.Success(changed);
    }
}