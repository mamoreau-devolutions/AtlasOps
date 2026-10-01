namespace AtlasOps.Features.Automation.ScheduledRunbook;

using AtlasOps.Features;

public sealed class ScheduledRunbookService(
    IAtlasOpsCapabilityRepository<ScheduledRunbookItem> repository,
    TimeProvider timeProvider)
{
    private readonly ScheduledRunbookValidator validator = new();
    private readonly ScheduledRunbookPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ScheduledRunbookChanged>> ExecuteAsync(
        UpdateScheduledRunbookCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ScheduledRunbookChanged>.Invalid(issues);
        }

        ScheduledRunbookItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ScheduledRunbookItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ScheduledRunbookChanged>.Invalid(
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

        ScheduledRunbookChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ScheduledRunbookChanged>.Success(changed);
    }
}