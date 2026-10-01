namespace AtlasOps.Features.Api.ApiDeploymentOptimization;

using AtlasOps.Features;

public sealed class ApiDeploymentOptimizationService(
    IAtlasOpsCapabilityRepository<ApiDeploymentOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiDeploymentOptimizationValidator validator = new();
    private readonly ApiDeploymentOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiDeploymentOptimizationChanged>> ExecuteAsync(
        UpdateApiDeploymentOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiDeploymentOptimizationChanged>.Invalid(issues);
        }

        ApiDeploymentOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiDeploymentOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiDeploymentOptimizationChanged>.Invalid(
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

        ApiDeploymentOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiDeploymentOptimizationChanged>.Success(changed);
    }
}