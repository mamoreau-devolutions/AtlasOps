namespace AtlasOps.Features.ServiceManagement.ServiceReviewMonitoring;

using AtlasOps.Features;

public sealed class ServiceReviewMonitoringService(
    IAtlasOpsCapabilityRepository<ServiceReviewMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceReviewMonitoringValidator validator = new();
    private readonly ServiceReviewMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceReviewMonitoringChanged>> ExecuteAsync(
        UpdateServiceReviewMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceReviewMonitoringChanged>.Invalid(issues);
        }

        ServiceReviewMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceReviewMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceReviewMonitoringChanged>.Invalid(
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

        ServiceReviewMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceReviewMonitoringChanged>.Success(changed);
    }
}