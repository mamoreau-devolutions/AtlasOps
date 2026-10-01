namespace AtlasOps.Features.Incidents.OnCallSchedule;

using AtlasOps.Features;

public sealed class OnCallScheduleService(
    IAtlasOpsCapabilityRepository<OnCallScheduleItem> repository,
    TimeProvider timeProvider)
{
    private readonly OnCallScheduleValidator validator = new();
    private readonly OnCallSchedulePolicy policy = new();

    public async Task<AtlasOpsOperationResult<OnCallScheduleChanged>> ExecuteAsync(
        UpdateOnCallScheduleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<OnCallScheduleChanged>.Invalid(issues);
        }

        OnCallScheduleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new OnCallScheduleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<OnCallScheduleChanged>.Invalid(
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

        OnCallScheduleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<OnCallScheduleChanged>.Success(changed);
    }
}