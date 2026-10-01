namespace AtlasOps.Features.Incidents.AlertSuppression;

using AtlasOps.Features;

public sealed class AlertSuppressionService(
    IAtlasOpsCapabilityRepository<AlertSuppressionItem> repository,
    TimeProvider timeProvider)
{
    private readonly AlertSuppressionValidator validator = new();
    private readonly AlertSuppressionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AlertSuppressionChanged>> ExecuteAsync(
        UpdateAlertSuppressionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AlertSuppressionChanged>.Invalid(issues);
        }

        AlertSuppressionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AlertSuppressionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AlertSuppressionChanged>.Invalid(
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

        AlertSuppressionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AlertSuppressionChanged>.Success(changed);
    }
}