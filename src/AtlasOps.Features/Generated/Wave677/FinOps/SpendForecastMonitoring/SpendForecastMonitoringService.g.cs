namespace AtlasOps.Features.FinOps.SpendForecastMonitoring;

using AtlasOps.Features;

public sealed class SpendForecastMonitoringService(
    IAtlasOpsCapabilityRepository<SpendForecastMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SpendForecastMonitoringValidator validator = new();
    private readonly SpendForecastMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SpendForecastMonitoringChanged>> ExecuteAsync(
        UpdateSpendForecastMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SpendForecastMonitoringChanged>.Invalid(issues);
        }

        SpendForecastMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SpendForecastMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SpendForecastMonitoringChanged>.Invalid(
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

        SpendForecastMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SpendForecastMonitoringChanged>.Success(changed);
    }
}