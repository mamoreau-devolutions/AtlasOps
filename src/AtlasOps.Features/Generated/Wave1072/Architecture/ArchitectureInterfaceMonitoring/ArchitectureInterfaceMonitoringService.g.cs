namespace AtlasOps.Features.Architecture.ArchitectureInterfaceMonitoring;

using AtlasOps.Features;

public sealed class ArchitectureInterfaceMonitoringService(
    IAtlasOpsCapabilityRepository<ArchitectureInterfaceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureInterfaceMonitoringValidator validator = new();
    private readonly ArchitectureInterfaceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureInterfaceMonitoringChanged>> ExecuteAsync(
        UpdateArchitectureInterfaceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureInterfaceMonitoringChanged>.Invalid(issues);
        }

        ArchitectureInterfaceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureInterfaceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureInterfaceMonitoringChanged>.Invalid(
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

        ArchitectureInterfaceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureInterfaceMonitoringChanged>.Success(changed);
    }
}