namespace AtlasOps.Features.FinOps.CostAllocationMonitoring;

using AtlasOps.Features;

public sealed class CostAllocationMonitoringService(
    IAtlasOpsCapabilityRepository<CostAllocationMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostAllocationMonitoringValidator validator = new();
    private readonly CostAllocationMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostAllocationMonitoringChanged>> ExecuteAsync(
        UpdateCostAllocationMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostAllocationMonitoringChanged>.Invalid(issues);
        }

        CostAllocationMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostAllocationMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostAllocationMonitoringChanged>.Invalid(
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

        CostAllocationMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostAllocationMonitoringChanged>.Success(changed);
    }
}