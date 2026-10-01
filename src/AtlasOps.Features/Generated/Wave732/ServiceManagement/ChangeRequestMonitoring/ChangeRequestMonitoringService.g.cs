namespace AtlasOps.Features.ServiceManagement.ChangeRequestMonitoring;

using AtlasOps.Features;

public sealed class ChangeRequestMonitoringService(
    IAtlasOpsCapabilityRepository<ChangeRequestMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChangeRequestMonitoringValidator validator = new();
    private readonly ChangeRequestMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChangeRequestMonitoringChanged>> ExecuteAsync(
        UpdateChangeRequestMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChangeRequestMonitoringChanged>.Invalid(issues);
        }

        ChangeRequestMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChangeRequestMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChangeRequestMonitoringChanged>.Invalid(
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

        ChangeRequestMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChangeRequestMonitoringChanged>.Success(changed);
    }
}