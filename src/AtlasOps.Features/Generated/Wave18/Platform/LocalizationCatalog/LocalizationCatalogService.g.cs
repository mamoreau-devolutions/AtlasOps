namespace AtlasOps.Features.Platform.LocalizationCatalog;

using AtlasOps.Features;

public sealed class LocalizationCatalogService(
    IAtlasOpsCapabilityRepository<LocalizationCatalogItem> repository,
    TimeProvider timeProvider)
{
    private readonly LocalizationCatalogValidator validator = new();
    private readonly LocalizationCatalogPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LocalizationCatalogChanged>> ExecuteAsync(
        UpdateLocalizationCatalogCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LocalizationCatalogChanged>.Invalid(issues);
        }

        LocalizationCatalogItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LocalizationCatalogItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LocalizationCatalogChanged>.Invalid(
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

        LocalizationCatalogChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LocalizationCatalogChanged>.Success(changed);
    }
}