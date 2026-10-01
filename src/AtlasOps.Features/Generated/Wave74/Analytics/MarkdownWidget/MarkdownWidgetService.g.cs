namespace AtlasOps.Features.Analytics.MarkdownWidget;

using AtlasOps.Features;

public sealed class MarkdownWidgetService(
    IAtlasOpsCapabilityRepository<MarkdownWidgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly MarkdownWidgetValidator validator = new();
    private readonly MarkdownWidgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MarkdownWidgetChanged>> ExecuteAsync(
        UpdateMarkdownWidgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MarkdownWidgetChanged>.Invalid(issues);
        }

        MarkdownWidgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MarkdownWidgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MarkdownWidgetChanged>.Invalid(
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

        MarkdownWidgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MarkdownWidgetChanged>.Success(changed);
    }
}