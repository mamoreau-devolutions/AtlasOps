namespace AtlasOps.Features.Platform.ClockAbstraction;

using AtlasOps.Features;

public sealed class ClockAbstractionService(
    IAtlasOpsCapabilityRepository<ClockAbstractionItem> repository,
    TimeProvider timeProvider)
{
    private readonly ClockAbstractionValidator validator = new();
    private readonly ClockAbstractionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ClockAbstractionChanged>> ExecuteAsync(
        UpdateClockAbstractionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ClockAbstractionChanged>.Invalid(issues);
        }

        ClockAbstractionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ClockAbstractionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ClockAbstractionChanged>.Invalid(
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

        ClockAbstractionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ClockAbstractionChanged>.Success(changed);
    }
}