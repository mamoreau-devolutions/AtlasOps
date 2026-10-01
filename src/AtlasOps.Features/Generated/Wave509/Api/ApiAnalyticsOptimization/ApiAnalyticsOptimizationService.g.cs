namespace AtlasOps.Features.Api.ApiAnalyticsOptimization;

using AtlasOps.Features;

public sealed class ApiAnalyticsOptimizationService(
    IAtlasOpsCapabilityRepository<ApiAnalyticsOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiAnalyticsOptimizationValidator validator = new();
    private readonly ApiAnalyticsOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiAnalyticsOptimizationChanged>> ExecuteAsync(
        UpdateApiAnalyticsOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiAnalyticsOptimizationChanged>.Invalid(issues);
        }

        ApiAnalyticsOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiAnalyticsOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiAnalyticsOptimizationChanged>.Invalid(
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

        ApiAnalyticsOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiAnalyticsOptimizationChanged>.Success(changed);
    }
}