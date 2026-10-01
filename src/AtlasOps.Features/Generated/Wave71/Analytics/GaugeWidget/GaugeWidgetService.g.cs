namespace AtlasOps.Features.Analytics.GaugeWidget;

using AtlasOps.Features;

public sealed class GaugeWidgetService(
    IAtlasOpsCapabilityRepository<GaugeWidgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly GaugeWidgetValidator validator = new();
    private readonly GaugeWidgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<GaugeWidgetChanged>> ExecuteAsync(
        UpdateGaugeWidgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<GaugeWidgetChanged>.Invalid(issues);
        }

        GaugeWidgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new GaugeWidgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<GaugeWidgetChanged>.Invalid(
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

        GaugeWidgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<GaugeWidgetChanged>.Success(changed);
    }
}