namespace AtlasOps.Features.ServiceManagement.ProblemRecordMonitoring;

using AtlasOps.Features;

public sealed class ProblemRecordMonitoringService(
    IAtlasOpsCapabilityRepository<ProblemRecordMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ProblemRecordMonitoringValidator validator = new();
    private readonly ProblemRecordMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ProblemRecordMonitoringChanged>> ExecuteAsync(
        UpdateProblemRecordMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ProblemRecordMonitoringChanged>.Invalid(issues);
        }

        ProblemRecordMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ProblemRecordMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ProblemRecordMonitoringChanged>.Invalid(
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

        ProblemRecordMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ProblemRecordMonitoringChanged>.Success(changed);
    }
}