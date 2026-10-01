namespace AtlasOps.Features.Incidents.IncidentCase;

using AtlasOps.Features;

public sealed class IncidentCaseService(
    IAtlasOpsCapabilityRepository<IncidentCaseItem> repository,
    TimeProvider timeProvider)
{
    private readonly IncidentCaseValidator validator = new();
    private readonly IncidentCasePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IncidentCaseChanged>> ExecuteAsync(
        UpdateIncidentCaseCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IncidentCaseChanged>.Invalid(issues);
        }

        IncidentCaseItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IncidentCaseItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IncidentCaseChanged>.Invalid(
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

        IncidentCaseChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IncidentCaseChanged>.Success(changed);
    }
}