namespace AtlasOps.Features.Analytics.HealthWidget;

using AtlasOps.Features;

public sealed class HealthWidgetService(
    IAtlasOpsCapabilityRepository<HealthWidgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly HealthWidgetValidator validator = new();
    private readonly HealthWidgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<HealthWidgetChanged>> ExecuteAsync(
        UpdateHealthWidgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<HealthWidgetChanged>.Invalid(issues);
        }

        HealthWidgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new HealthWidgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<HealthWidgetChanged>.Invalid(
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

        HealthWidgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<HealthWidgetChanged>.Success(changed);
    }
}