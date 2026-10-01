namespace AtlasOps.Features.Api.ApiTokenOptimization;

using AtlasOps.Features;

public sealed class ApiTokenOptimizationService(
    IAtlasOpsCapabilityRepository<ApiTokenOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiTokenOptimizationValidator validator = new();
    private readonly ApiTokenOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiTokenOptimizationChanged>> ExecuteAsync(
        UpdateApiTokenOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiTokenOptimizationChanged>.Invalid(issues);
        }

        ApiTokenOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiTokenOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiTokenOptimizationChanged>.Invalid(
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

        ApiTokenOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiTokenOptimizationChanged>.Success(changed);
    }
}