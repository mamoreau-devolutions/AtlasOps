namespace AtlasOps.Features.Incidents.IncidentNotification;

using AtlasOps.Features;

public sealed class IncidentNotificationService(
    IAtlasOpsCapabilityRepository<IncidentNotificationItem> repository,
    TimeProvider timeProvider)
{
    private readonly IncidentNotificationValidator validator = new();
    private readonly IncidentNotificationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IncidentNotificationChanged>> ExecuteAsync(
        UpdateIncidentNotificationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IncidentNotificationChanged>.Invalid(issues);
        }

        IncidentNotificationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IncidentNotificationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IncidentNotificationChanged>.Invalid(
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

        IncidentNotificationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IncidentNotificationChanged>.Success(changed);
    }
}