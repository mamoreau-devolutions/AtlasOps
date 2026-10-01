namespace AtlasOps.Features.Api.ApiGatewayMonitoring;

using AtlasOps.Features;

public sealed class ApiGatewayMonitoringService(
    IAtlasOpsCapabilityRepository<ApiGatewayMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiGatewayMonitoringValidator validator = new();
    private readonly ApiGatewayMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiGatewayMonitoringChanged>> ExecuteAsync(
        UpdateApiGatewayMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiGatewayMonitoringChanged>.Invalid(issues);
        }

        ApiGatewayMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiGatewayMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiGatewayMonitoringChanged>.Invalid(
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

        ApiGatewayMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiGatewayMonitoringChanged>.Success(changed);
    }
}