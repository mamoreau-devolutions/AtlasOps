namespace AtlasOps.Features.Architecture.ArchitectureStandardMonitoring;

using AtlasOps.Features;

public sealed class ArchitectureStandardMonitoringService(
    IAtlasOpsCapabilityRepository<ArchitectureStandardMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureStandardMonitoringValidator validator = new();
    private readonly ArchitectureStandardMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureStandardMonitoringChanged>> ExecuteAsync(
        UpdateArchitectureStandardMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureStandardMonitoringChanged>.Invalid(issues);
        }

        ArchitectureStandardMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureStandardMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureStandardMonitoringChanged>.Invalid(
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

        ArchitectureStandardMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureStandardMonitoringChanged>.Success(changed);
    }
}