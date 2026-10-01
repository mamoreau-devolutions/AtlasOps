namespace AtlasOps.Features.FinOps.CostCenterMonitoring;

using AtlasOps.Features;

public sealed class CostCenterMonitoringService(
    IAtlasOpsCapabilityRepository<CostCenterMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostCenterMonitoringValidator validator = new();
    private readonly CostCenterMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostCenterMonitoringChanged>> ExecuteAsync(
        UpdateCostCenterMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostCenterMonitoringChanged>.Invalid(issues);
        }

        CostCenterMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostCenterMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostCenterMonitoringChanged>.Invalid(
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

        CostCenterMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostCenterMonitoringChanged>.Success(changed);
    }
}