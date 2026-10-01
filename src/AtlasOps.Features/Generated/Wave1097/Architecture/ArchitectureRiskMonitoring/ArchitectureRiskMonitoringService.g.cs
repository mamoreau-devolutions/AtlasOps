namespace AtlasOps.Features.Architecture.ArchitectureRiskMonitoring;

using AtlasOps.Features;

public sealed class ArchitectureRiskMonitoringService(
    IAtlasOpsCapabilityRepository<ArchitectureRiskMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRiskMonitoringValidator validator = new();
    private readonly ArchitectureRiskMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRiskMonitoringChanged>> ExecuteAsync(
        UpdateArchitectureRiskMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRiskMonitoringChanged>.Invalid(issues);
        }

        ArchitectureRiskMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRiskMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRiskMonitoringChanged>.Invalid(
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

        ArchitectureRiskMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRiskMonitoringChanged>.Success(changed);
    }
}