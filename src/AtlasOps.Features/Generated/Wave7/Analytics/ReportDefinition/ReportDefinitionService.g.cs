namespace AtlasOps.Features.Analytics.ReportDefinition;

using AtlasOps.Features;

public sealed class ReportDefinitionService(
    IAtlasOpsCapabilityRepository<ReportDefinitionItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReportDefinitionValidator validator = new();
    private readonly ReportDefinitionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReportDefinitionChanged>> ExecuteAsync(
        UpdateReportDefinitionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReportDefinitionChanged>.Invalid(issues);
        }

        ReportDefinitionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReportDefinitionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReportDefinitionChanged>.Invalid(
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

        ReportDefinitionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReportDefinitionChanged>.Success(changed);
    }
}