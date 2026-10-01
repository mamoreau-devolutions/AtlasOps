namespace AtlasOps.Features.Api.ApiHealthOptimization;

using AtlasOps.Features;

public sealed class ApiHealthOptimizationService(
    IAtlasOpsCapabilityRepository<ApiHealthOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiHealthOptimizationValidator validator = new();
    private readonly ApiHealthOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiHealthOptimizationChanged>> ExecuteAsync(
        UpdateApiHealthOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiHealthOptimizationChanged>.Invalid(issues);
        }

        ApiHealthOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiHealthOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiHealthOptimizationChanged>.Invalid(
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

        ApiHealthOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiHealthOptimizationChanged>.Success(changed);
    }
}