namespace AtlasOps.Features.Analytics.NumberWidget;

using AtlasOps.Features;

public sealed class NumberWidgetService(
    IAtlasOpsCapabilityRepository<NumberWidgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly NumberWidgetValidator validator = new();
    private readonly NumberWidgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NumberWidgetChanged>> ExecuteAsync(
        UpdateNumberWidgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NumberWidgetChanged>.Invalid(issues);
        }

        NumberWidgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NumberWidgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NumberWidgetChanged>.Invalid(
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

        NumberWidgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NumberWidgetChanged>.Success(changed);
    }
}