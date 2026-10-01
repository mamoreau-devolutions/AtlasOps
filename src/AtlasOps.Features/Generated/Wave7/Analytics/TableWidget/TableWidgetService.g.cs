namespace AtlasOps.Features.Analytics.TableWidget;

using AtlasOps.Features;

public sealed class TableWidgetService(
    IAtlasOpsCapabilityRepository<TableWidgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly TableWidgetValidator validator = new();
    private readonly TableWidgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TableWidgetChanged>> ExecuteAsync(
        UpdateTableWidgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TableWidgetChanged>.Invalid(issues);
        }

        TableWidgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TableWidgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TableWidgetChanged>.Invalid(
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

        TableWidgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TableWidgetChanged>.Success(changed);
    }
}