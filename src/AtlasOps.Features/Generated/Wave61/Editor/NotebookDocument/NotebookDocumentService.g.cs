namespace AtlasOps.Features.Editor.NotebookDocument;

using AtlasOps.Features;

public sealed class NotebookDocumentService(
    IAtlasOpsCapabilityRepository<NotebookDocumentItem> repository,
    TimeProvider timeProvider)
{
    private readonly NotebookDocumentValidator validator = new();
    private readonly NotebookDocumentPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NotebookDocumentChanged>> ExecuteAsync(
        UpdateNotebookDocumentCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NotebookDocumentChanged>.Invalid(issues);
        }

        NotebookDocumentItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NotebookDocumentItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NotebookDocumentChanged>.Invalid(
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

        NotebookDocumentChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NotebookDocumentChanged>.Success(changed);
    }
}