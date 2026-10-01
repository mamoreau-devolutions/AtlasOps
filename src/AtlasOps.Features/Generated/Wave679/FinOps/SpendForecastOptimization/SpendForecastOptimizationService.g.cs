namespace AtlasOps.Features.FinOps.SpendForecastOptimization;

using AtlasOps.Features;

public sealed class SpendForecastOptimizationService(
    IAtlasOpsCapabilityRepository<SpendForecastOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SpendForecastOptimizationValidator validator = new();
    private readonly SpendForecastOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SpendForecastOptimizationChanged>> ExecuteAsync(
        UpdateSpendForecastOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SpendForecastOptimizationChanged>.Invalid(issues);
        }

        SpendForecastOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SpendForecastOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SpendForecastOptimizationChanged>.Invalid(
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

        SpendForecastOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SpendForecastOptimizationChanged>.Success(changed);
    }
}