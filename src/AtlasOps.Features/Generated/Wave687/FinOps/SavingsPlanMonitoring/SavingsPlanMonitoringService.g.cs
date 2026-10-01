namespace AtlasOps.Features.FinOps.SavingsPlanMonitoring;

using AtlasOps.Features;

public sealed class SavingsPlanMonitoringService(
    IAtlasOpsCapabilityRepository<SavingsPlanMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SavingsPlanMonitoringValidator validator = new();
    private readonly SavingsPlanMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SavingsPlanMonitoringChanged>> ExecuteAsync(
        UpdateSavingsPlanMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SavingsPlanMonitoringChanged>.Invalid(issues);
        }

        SavingsPlanMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SavingsPlanMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SavingsPlanMonitoringChanged>.Invalid(
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

        SavingsPlanMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SavingsPlanMonitoringChanged>.Success(changed);
    }
}