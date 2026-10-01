namespace AtlasOps.Features.Api.ApiClientMonitoring;

using AtlasOps.Features;

public sealed class ApiClientMonitoringService(
    IAtlasOpsCapabilityRepository<ApiClientMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiClientMonitoringValidator validator = new();
    private readonly ApiClientMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiClientMonitoringChanged>> ExecuteAsync(
        UpdateApiClientMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiClientMonitoringChanged>.Invalid(issues);
        }

        ApiClientMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiClientMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiClientMonitoringChanged>.Invalid(
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

        ApiClientMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiClientMonitoringChanged>.Success(changed);
    }
}