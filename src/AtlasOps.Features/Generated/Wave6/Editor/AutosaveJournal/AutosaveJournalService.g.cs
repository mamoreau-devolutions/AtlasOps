namespace AtlasOps.Features.Editor.AutosaveJournal;

using AtlasOps.Features;

public sealed class AutosaveJournalService(
    IAtlasOpsCapabilityRepository<AutosaveJournalItem> repository,
    TimeProvider timeProvider)
{
    private readonly AutosaveJournalValidator validator = new();
    private readonly AutosaveJournalPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AutosaveJournalChanged>> ExecuteAsync(
        UpdateAutosaveJournalCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AutosaveJournalChanged>.Invalid(issues);
        }

        AutosaveJournalItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AutosaveJournalItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AutosaveJournalChanged>.Invalid(
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

        AutosaveJournalChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AutosaveJournalChanged>.Success(changed);
    }
}