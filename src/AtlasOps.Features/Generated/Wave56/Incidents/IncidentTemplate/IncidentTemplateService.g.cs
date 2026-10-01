namespace AtlasOps.Features.Incidents.IncidentTemplate;

using AtlasOps.Features;

public sealed class IncidentTemplateService(
    IAtlasOpsCapabilityRepository<IncidentTemplateItem> repository,
    TimeProvider timeProvider)
{
    private readonly IncidentTemplateValidator validator = new();
    private readonly IncidentTemplatePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IncidentTemplateChanged>> ExecuteAsync(
        UpdateIncidentTemplateCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IncidentTemplateChanged>.Invalid(issues);
        }

        IncidentTemplateItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IncidentTemplateItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IncidentTemplateChanged>.Invalid(
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

        IncidentTemplateChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IncidentTemplateChanged>.Success(changed);
    }
}