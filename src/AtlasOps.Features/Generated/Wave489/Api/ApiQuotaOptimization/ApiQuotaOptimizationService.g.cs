namespace AtlasOps.Features.Api.ApiQuotaOptimization;

using AtlasOps.Features;

public sealed class ApiQuotaOptimizationService(
    IAtlasOpsCapabilityRepository<ApiQuotaOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiQuotaOptimizationValidator validator = new();
    private readonly ApiQuotaOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiQuotaOptimizationChanged>> ExecuteAsync(
        UpdateApiQuotaOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiQuotaOptimizationChanged>.Invalid(issues);
        }

        ApiQuotaOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiQuotaOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiQuotaOptimizationChanged>.Invalid(
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

        ApiQuotaOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiQuotaOptimizationChanged>.Success(changed);
    }
}