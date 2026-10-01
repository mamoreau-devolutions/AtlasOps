namespace AtlasOps.Features.Api.ApiAnalyticsMonitoring;

using AtlasOps.Features;

public sealed class ApiAnalyticsMonitoringService(
    IAtlasOpsCapabilityRepository<ApiAnalyticsMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiAnalyticsMonitoringValidator validator = new();
    private readonly ApiAnalyticsMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiAnalyticsMonitoringChanged>> ExecuteAsync(
        UpdateApiAnalyticsMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiAnalyticsMonitoringChanged>.Invalid(issues);
        }

        ApiAnalyticsMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiAnalyticsMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiAnalyticsMonitoringChanged>.Invalid(
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

        ApiAnalyticsMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiAnalyticsMonitoringChanged>.Success(changed);
    }
}