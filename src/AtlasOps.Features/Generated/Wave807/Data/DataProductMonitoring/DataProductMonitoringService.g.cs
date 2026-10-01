namespace AtlasOps.Features.Data.DataProductMonitoring;

using AtlasOps.Features;

public sealed class DataProductMonitoringService(
    IAtlasOpsCapabilityRepository<DataProductMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataProductMonitoringValidator validator = new();
    private readonly DataProductMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataProductMonitoringChanged>> ExecuteAsync(
        UpdateDataProductMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataProductMonitoringChanged>.Invalid(issues);
        }

        DataProductMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataProductMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataProductMonitoringChanged>.Invalid(
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

        DataProductMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataProductMonitoringChanged>.Success(changed);
    }
}