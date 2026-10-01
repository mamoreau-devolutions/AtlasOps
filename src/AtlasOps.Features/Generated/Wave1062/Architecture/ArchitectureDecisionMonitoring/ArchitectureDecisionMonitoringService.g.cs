namespace AtlasOps.Features.Architecture.ArchitectureDecisionMonitoring;

using AtlasOps.Features;

public sealed class ArchitectureDecisionMonitoringService(
    IAtlasOpsCapabilityRepository<ArchitectureDecisionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDecisionMonitoringValidator validator = new();
    private readonly ArchitectureDecisionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDecisionMonitoringChanged>> ExecuteAsync(
        UpdateArchitectureDecisionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDecisionMonitoringChanged>.Invalid(issues);
        }

        ArchitectureDecisionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDecisionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDecisionMonitoringChanged>.Invalid(
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

        ArchitectureDecisionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDecisionMonitoringChanged>.Success(changed);
    }
}