namespace AtlasOps.Features.Platform.NotificationDelivery;

using AtlasOps.Features;

public sealed class NotificationDeliveryService(
    IAtlasOpsCapabilityRepository<NotificationDeliveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NotificationDeliveryValidator validator = new();
    private readonly NotificationDeliveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NotificationDeliveryChanged>> ExecuteAsync(
        UpdateNotificationDeliveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NotificationDeliveryChanged>.Invalid(issues);
        }

        NotificationDeliveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NotificationDeliveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NotificationDeliveryChanged>.Invalid(
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

        NotificationDeliveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NotificationDeliveryChanged>.Success(changed);
    }
}