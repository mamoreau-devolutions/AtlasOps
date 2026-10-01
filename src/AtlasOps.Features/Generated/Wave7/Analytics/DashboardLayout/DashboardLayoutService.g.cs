namespace AtlasOps.Features.Analytics.DashboardLayout;

using AtlasOps.Features;

public sealed class DashboardLayoutService(
    IAtlasOpsCapabilityRepository<DashboardLayoutItem> repository,
    TimeProvider timeProvider)
{
    private readonly DashboardLayoutValidator validator = new();
    private readonly DashboardLayoutPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DashboardLayoutChanged>> ExecuteAsync(
        UpdateDashboardLayoutCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DashboardLayoutChanged>.Invalid(issues);
        }

        DashboardLayoutItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DashboardLayoutItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DashboardLayoutChanged>.Invalid(
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

        DashboardLayoutChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DashboardLayoutChanged>.Success(changed);
    }
}