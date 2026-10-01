namespace AtlasOps.Features.Api.ApiEndpointOptimization;

using AtlasOps.Features;

public sealed class ApiEndpointOptimizationService(
    IAtlasOpsCapabilityRepository<ApiEndpointOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiEndpointOptimizationValidator validator = new();
    private readonly ApiEndpointOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiEndpointOptimizationChanged>> ExecuteAsync(
        UpdateApiEndpointOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiEndpointOptimizationChanged>.Invalid(issues);
        }

        ApiEndpointOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiEndpointOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiEndpointOptimizationChanged>.Invalid(
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

        ApiEndpointOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiEndpointOptimizationChanged>.Success(changed);
    }
}