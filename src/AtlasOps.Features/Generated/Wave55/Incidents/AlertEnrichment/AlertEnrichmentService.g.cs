namespace AtlasOps.Features.Incidents.AlertEnrichment;

using AtlasOps.Features;

public sealed class AlertEnrichmentService(
    IAtlasOpsCapabilityRepository<AlertEnrichmentItem> repository,
    TimeProvider timeProvider)
{
    private readonly AlertEnrichmentValidator validator = new();
    private readonly AlertEnrichmentPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AlertEnrichmentChanged>> ExecuteAsync(
        UpdateAlertEnrichmentCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AlertEnrichmentChanged>.Invalid(issues);
        }

        AlertEnrichmentItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AlertEnrichmentItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AlertEnrichmentChanged>.Invalid(
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

        AlertEnrichmentChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AlertEnrichmentChanged>.Success(changed);
    }
}