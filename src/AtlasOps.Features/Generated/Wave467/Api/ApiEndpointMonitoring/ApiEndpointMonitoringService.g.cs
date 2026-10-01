namespace AtlasOps.Features.Api.ApiEndpointMonitoring;

using AtlasOps.Features;

public sealed class ApiEndpointMonitoringService(
    IAtlasOpsCapabilityRepository<ApiEndpointMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiEndpointMonitoringValidator validator = new();
    private readonly ApiEndpointMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiEndpointMonitoringChanged>> ExecuteAsync(
        UpdateApiEndpointMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiEndpointMonitoringChanged>.Invalid(issues);
        }

        ApiEndpointMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiEndpointMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiEndpointMonitoringChanged>.Invalid(
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

        ApiEndpointMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiEndpointMonitoringChanged>.Success(changed);
    }
}