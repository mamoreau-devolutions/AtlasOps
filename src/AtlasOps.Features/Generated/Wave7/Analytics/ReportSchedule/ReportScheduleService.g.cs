namespace AtlasOps.Features.Analytics.ReportSchedule;

using AtlasOps.Features;

public sealed class ReportScheduleService(
    IAtlasOpsCapabilityRepository<ReportScheduleItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReportScheduleValidator validator = new();
    private readonly ReportSchedulePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReportScheduleChanged>> ExecuteAsync(
        UpdateReportScheduleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReportScheduleChanged>.Invalid(issues);
        }

        ReportScheduleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReportScheduleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReportScheduleChanged>.Invalid(
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

        ReportScheduleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReportScheduleChanged>.Success(changed);
    }
}