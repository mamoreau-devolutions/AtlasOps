namespace AtlasOps.Features.Api.ApiTokenMonitoring;

using AtlasOps.Features;

public sealed class ApiTokenMonitoringService(
    IAtlasOpsCapabilityRepository<ApiTokenMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiTokenMonitoringValidator validator = new();
    private readonly ApiTokenMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiTokenMonitoringChanged>> ExecuteAsync(
        UpdateApiTokenMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiTokenMonitoringChanged>.Invalid(issues);
        }

        ApiTokenMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiTokenMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiTokenMonitoringChanged>.Invalid(
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

        ApiTokenMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiTokenMonitoringChanged>.Success(changed);
    }
}