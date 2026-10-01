namespace AtlasOps.Features.Api.ApiClientOptimization;

using AtlasOps.Features;

public sealed class ApiClientOptimizationService(
    IAtlasOpsCapabilityRepository<ApiClientOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiClientOptimizationValidator validator = new();
    private readonly ApiClientOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiClientOptimizationChanged>> ExecuteAsync(
        UpdateApiClientOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiClientOptimizationChanged>.Invalid(issues);
        }

        ApiClientOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiClientOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiClientOptimizationChanged>.Invalid(
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

        ApiClientOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiClientOptimizationChanged>.Success(changed);
    }
}