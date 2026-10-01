namespace AtlasOps.Features.Sync.TombstoneRecord;

using AtlasOps.Features;

public sealed class TombstoneRecordService(
    IAtlasOpsCapabilityRepository<TombstoneRecordItem> repository,
    TimeProvider timeProvider)
{
    private readonly TombstoneRecordValidator validator = new();
    private readonly TombstoneRecordPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TombstoneRecordChanged>> ExecuteAsync(
        UpdateTombstoneRecordCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TombstoneRecordChanged>.Invalid(issues);
        }

        TombstoneRecordItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TombstoneRecordItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TombstoneRecordChanged>.Invalid(
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

        TombstoneRecordChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TombstoneRecordChanged>.Success(changed);
    }
}