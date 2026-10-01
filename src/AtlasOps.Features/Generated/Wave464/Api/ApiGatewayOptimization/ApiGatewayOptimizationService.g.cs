namespace AtlasOps.Features.Api.ApiGatewayOptimization;

using AtlasOps.Features;

public sealed class ApiGatewayOptimizationService(
    IAtlasOpsCapabilityRepository<ApiGatewayOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiGatewayOptimizationValidator validator = new();
    private readonly ApiGatewayOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiGatewayOptimizationChanged>> ExecuteAsync(
        UpdateApiGatewayOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiGatewayOptimizationChanged>.Invalid(issues);
        }

        ApiGatewayOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiGatewayOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiGatewayOptimizationChanged>.Invalid(
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

        ApiGatewayOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiGatewayOptimizationChanged>.Success(changed);
    }
}