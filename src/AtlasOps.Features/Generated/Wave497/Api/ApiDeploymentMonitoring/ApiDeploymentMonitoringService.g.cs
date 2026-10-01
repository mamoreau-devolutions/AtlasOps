namespace AtlasOps.Features.Api.ApiDeploymentMonitoring;

using AtlasOps.Features;

public sealed class ApiDeploymentMonitoringService(
    IAtlasOpsCapabilityRepository<ApiDeploymentMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiDeploymentMonitoringValidator validator = new();
    private readonly ApiDeploymentMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiDeploymentMonitoringChanged>> ExecuteAsync(
        UpdateApiDeploymentMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiDeploymentMonitoringChanged>.Invalid(issues);
        }

        ApiDeploymentMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiDeploymentMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiDeploymentMonitoringChanged>.Invalid(
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

        ApiDeploymentMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiDeploymentMonitoringChanged>.Success(changed);
    }
}