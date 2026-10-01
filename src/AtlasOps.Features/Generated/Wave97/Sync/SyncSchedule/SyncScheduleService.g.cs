namespace AtlasOps.Features.Sync.SyncSchedule;

using AtlasOps.Features;

public sealed class SyncScheduleService(
    IAtlasOpsCapabilityRepository<SyncScheduleItem> repository,
    TimeProvider timeProvider)
{
    private readonly SyncScheduleValidator validator = new();
    private readonly SyncSchedulePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SyncScheduleChanged>> ExecuteAsync(
        UpdateSyncScheduleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SyncScheduleChanged>.Invalid(issues);
        }

        SyncScheduleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SyncScheduleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SyncScheduleChanged>.Invalid(
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

        SyncScheduleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SyncScheduleChanged>.Success(changed);
    }
}