namespace AtlasOps.Features.Editor.ScriptLibrary;

using AtlasOps.Features;

public sealed class ScriptLibraryService(
    IAtlasOpsCapabilityRepository<ScriptLibraryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ScriptLibraryValidator validator = new();
    private readonly ScriptLibraryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ScriptLibraryChanged>> ExecuteAsync(
        UpdateScriptLibraryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ScriptLibraryChanged>.Invalid(issues);
        }

        ScriptLibraryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ScriptLibraryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ScriptLibraryChanged>.Invalid(
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

        ScriptLibraryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ScriptLibraryChanged>.Success(changed);
    }
}