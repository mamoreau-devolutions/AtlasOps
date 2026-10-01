namespace AtlasOps.Features.Platform.ThemeComposition;

using AtlasOps.Features;

public sealed class ThemeCompositionService(
    IAtlasOpsCapabilityRepository<ThemeCompositionItem> repository,
    TimeProvider timeProvider)
{
    private readonly ThemeCompositionValidator validator = new();
    private readonly ThemeCompositionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ThemeCompositionChanged>> ExecuteAsync(
        UpdateThemeCompositionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ThemeCompositionChanged>.Invalid(issues);
        }

        ThemeCompositionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ThemeCompositionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ThemeCompositionChanged>.Invalid(
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

        ThemeCompositionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ThemeCompositionChanged>.Success(changed);
    }
}