namespace AtlasOps.Features.FinOps.ChargebackRuleOptimization;

using AtlasOps.Features;

public sealed class ChargebackRuleOptimizationService(
    IAtlasOpsCapabilityRepository<ChargebackRuleOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChargebackRuleOptimizationValidator validator = new();
    private readonly ChargebackRuleOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChargebackRuleOptimizationChanged>> ExecuteAsync(
        UpdateChargebackRuleOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChargebackRuleOptimizationChanged>.Invalid(issues);
        }

        ChargebackRuleOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChargebackRuleOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChargebackRuleOptimizationChanged>.Invalid(
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

        ChargebackRuleOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChargebackRuleOptimizationChanged>.Success(changed);
    }
}