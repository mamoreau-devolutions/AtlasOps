namespace AtlasOps.Features.Architecture.ArchitectureComponentMonitoring;

using AtlasOps.Features;

public sealed class ArchitectureComponentMonitoringService(
    IAtlasOpsCapabilityRepository<ArchitectureComponentMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureComponentMonitoringValidator validator = new();
    private readonly ArchitectureComponentMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureComponentMonitoringChanged>> ExecuteAsync(
        UpdateArchitectureComponentMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureComponentMonitoringChanged>.Invalid(issues);
        }

        ArchitectureComponentMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureComponentMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureComponentMonitoringChanged>.Invalid(
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

        ArchitectureComponentMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureComponentMonitoringChanged>.Success(changed);
    }
}