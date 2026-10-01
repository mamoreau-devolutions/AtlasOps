namespace AtlasOps.Features.FinOps.ChargebackRuleMonitoring;

using AtlasOps.Features;

public sealed class ChargebackRuleMonitoringService(
    IAtlasOpsCapabilityRepository<ChargebackRuleMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChargebackRuleMonitoringValidator validator = new();
    private readonly ChargebackRuleMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChargebackRuleMonitoringChanged>> ExecuteAsync(
        UpdateChargebackRuleMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChargebackRuleMonitoringChanged>.Invalid(issues);
        }

        ChargebackRuleMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChargebackRuleMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChargebackRuleMonitoringChanged>.Invalid(
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

        ChargebackRuleMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChargebackRuleMonitoringChanged>.Success(changed);
    }
}