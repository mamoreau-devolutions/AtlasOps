namespace AtlasOps.Features.Incidents.AlertRule;

using AtlasOps.Features;

public sealed class AlertRuleService(
    IAtlasOpsCapabilityRepository<AlertRuleItem> repository,
    TimeProvider timeProvider)
{
    private readonly AlertRuleValidator validator = new();
    private readonly AlertRulePolicy policy = new();

    public async Task<AtlasOpsOperationResult<AlertRuleChanged>> ExecuteAsync(
        UpdateAlertRuleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AlertRuleChanged>.Invalid(issues);
        }

        AlertRuleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AlertRuleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AlertRuleChanged>.Invalid(
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

        AlertRuleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AlertRuleChanged>.Success(changed);
    }
}