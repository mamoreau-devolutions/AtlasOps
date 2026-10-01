namespace AtlasOps.Features.Architecture.ArchitectureExceptionMonitoring;

using AtlasOps.Features;

public sealed class ArchitectureExceptionMonitoringService(
    IAtlasOpsCapabilityRepository<ArchitectureExceptionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureExceptionMonitoringValidator validator = new();
    private readonly ArchitectureExceptionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureExceptionMonitoringChanged>> ExecuteAsync(
        UpdateArchitectureExceptionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureExceptionMonitoringChanged>.Invalid(issues);
        }

        ArchitectureExceptionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureExceptionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureExceptionMonitoringChanged>.Invalid(
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

        ArchitectureExceptionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureExceptionMonitoringChanged>.Success(changed);
    }
}