namespace AtlasOps.Features.Editor.SnippetCatalog;

using AtlasOps.Features;

public sealed class SnippetCatalogService(
    IAtlasOpsCapabilityRepository<SnippetCatalogItem> repository,
    TimeProvider timeProvider)
{
    private readonly SnippetCatalogValidator validator = new();
    private readonly SnippetCatalogPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SnippetCatalogChanged>> ExecuteAsync(
        UpdateSnippetCatalogCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SnippetCatalogChanged>.Invalid(issues);
        }

        SnippetCatalogItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SnippetCatalogItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SnippetCatalogChanged>.Invalid(
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

        SnippetCatalogChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SnippetCatalogChanged>.Success(changed);
    }
}