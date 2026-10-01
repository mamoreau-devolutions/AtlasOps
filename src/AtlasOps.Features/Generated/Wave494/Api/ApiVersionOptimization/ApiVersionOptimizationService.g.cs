namespace AtlasOps.Features.Api.ApiVersionOptimization;

using AtlasOps.Features;

public sealed class ApiVersionOptimizationService(
    IAtlasOpsCapabilityRepository<ApiVersionOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiVersionOptimizationValidator validator = new();
    private readonly ApiVersionOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiVersionOptimizationChanged>> ExecuteAsync(
        UpdateApiVersionOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiVersionOptimizationChanged>.Invalid(issues);
        }

        ApiVersionOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiVersionOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiVersionOptimizationChanged>.Invalid(
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

        ApiVersionOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiVersionOptimizationChanged>.Success(changed);
    }
}