namespace AtlasOps.Features.Incidents.AlertCorrelation;

using AtlasOps.Features;

public sealed class AlertCorrelationService(
    IAtlasOpsCapabilityRepository<AlertCorrelationItem> repository,
    TimeProvider timeProvider)
{
    private readonly AlertCorrelationValidator validator = new();
    private readonly AlertCorrelationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AlertCorrelationChanged>> ExecuteAsync(
        UpdateAlertCorrelationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AlertCorrelationChanged>.Invalid(issues);
        }

        AlertCorrelationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AlertCorrelationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AlertCorrelationChanged>.Invalid(
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

        AlertCorrelationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AlertCorrelationChanged>.Success(changed);
    }
}