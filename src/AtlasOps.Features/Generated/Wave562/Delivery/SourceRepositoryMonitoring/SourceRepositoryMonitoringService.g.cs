namespace AtlasOps.Features.Delivery.SourceRepositoryMonitoring;

using AtlasOps.Features;

public sealed class SourceRepositoryMonitoringService(
    IAtlasOpsCapabilityRepository<SourceRepositoryMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SourceRepositoryMonitoringValidator validator = new();
    private readonly SourceRepositoryMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SourceRepositoryMonitoringChanged>> ExecuteAsync(
        UpdateSourceRepositoryMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SourceRepositoryMonitoringChanged>.Invalid(issues);
        }

        SourceRepositoryMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SourceRepositoryMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SourceRepositoryMonitoringChanged>.Invalid(
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

        SourceRepositoryMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SourceRepositoryMonitoringChanged>.Success(changed);
    }
}