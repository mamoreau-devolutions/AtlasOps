namespace AtlasOps.Features.FinOps.ChargebackRuleRecovery;

using AtlasOps.Features;

public sealed class ChargebackRuleRecoveryService(
    IAtlasOpsCapabilityRepository<ChargebackRuleRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChargebackRuleRecoveryValidator validator = new();
    private readonly ChargebackRuleRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChargebackRuleRecoveryChanged>> ExecuteAsync(
        UpdateChargebackRuleRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChargebackRuleRecoveryChanged>.Invalid(issues);
        }

        ChargebackRuleRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChargebackRuleRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChargebackRuleRecoveryChanged>.Invalid(
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

        ChargebackRuleRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChargebackRuleRecoveryChanged>.Success(changed);
    }
}