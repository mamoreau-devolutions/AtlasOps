namespace AtlasOps.Features.Delivery.ReleaseCalendarGovernance;

using AtlasOps.Features;

public sealed class ReleaseCalendarGovernanceService(
    IAtlasOpsCapabilityRepository<ReleaseCalendarGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseCalendarGovernanceValidator validator = new();
    private readonly ReleaseCalendarGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseCalendarGovernanceChanged>> ExecuteAsync(
        UpdateReleaseCalendarGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseCalendarGovernanceChanged>.Invalid(issues);
        }

        ReleaseCalendarGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseCalendarGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseCalendarGovernanceChanged>.Invalid(
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

        ReleaseCalendarGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseCalendarGovernanceChanged>.Success(changed);
    }
}