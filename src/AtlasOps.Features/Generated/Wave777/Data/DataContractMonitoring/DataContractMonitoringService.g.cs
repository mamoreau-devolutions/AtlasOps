namespace AtlasOps.Features.Data.DataContractMonitoring;

using AtlasOps.Features;

public sealed class DataContractMonitoringService(
    IAtlasOpsCapabilityRepository<DataContractMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataContractMonitoringValidator validator = new();
    private readonly DataContractMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataContractMonitoringChanged>> ExecuteAsync(
        UpdateDataContractMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataContractMonitoringChanged>.Invalid(issues);
        }

        DataContractMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataContractMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataContractMonitoringChanged>.Invalid(
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

        DataContractMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataContractMonitoringChanged>.Success(changed);
    }
}