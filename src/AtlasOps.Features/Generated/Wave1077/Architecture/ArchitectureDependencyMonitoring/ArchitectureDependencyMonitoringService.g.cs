namespace AtlasOps.Features.Architecture.ArchitectureDependencyMonitoring;

using AtlasOps.Features;

public sealed class ArchitectureDependencyMonitoringService(
    IAtlasOpsCapabilityRepository<ArchitectureDependencyMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDependencyMonitoringValidator validator = new();
    private readonly ArchitectureDependencyMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDependencyMonitoringChanged>> ExecuteAsync(
        UpdateArchitectureDependencyMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDependencyMonitoringChanged>.Invalid(issues);
        }

        ArchitectureDependencyMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDependencyMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDependencyMonitoringChanged>.Invalid(
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

        ArchitectureDependencyMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDependencyMonitoringChanged>.Success(changed);
    }
}