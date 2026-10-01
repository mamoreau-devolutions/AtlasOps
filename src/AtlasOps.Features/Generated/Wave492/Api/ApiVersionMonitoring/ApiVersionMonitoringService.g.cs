namespace AtlasOps.Features.Api.ApiVersionMonitoring;

using AtlasOps.Features;

public sealed class ApiVersionMonitoringService(
    IAtlasOpsCapabilityRepository<ApiVersionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiVersionMonitoringValidator validator = new();
    private readonly ApiVersionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiVersionMonitoringChanged>> ExecuteAsync(
        UpdateApiVersionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiVersionMonitoringChanged>.Invalid(issues);
        }

        ApiVersionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiVersionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiVersionMonitoringChanged>.Invalid(
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

        ApiVersionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiVersionMonitoringChanged>.Success(changed);
    }
}