namespace AtlasOps.Features.Governance.TeamMembership;

using AtlasOps.Features;

public sealed class TeamMembershipService(
    IAtlasOpsCapabilityRepository<TeamMembershipItem> repository,
    TimeProvider timeProvider)
{
    private readonly TeamMembershipValidator validator = new();
    private readonly TeamMembershipPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TeamMembershipChanged>> ExecuteAsync(
        UpdateTeamMembershipCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TeamMembershipChanged>.Invalid(issues);
        }

        TeamMembershipItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TeamMembershipItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TeamMembershipChanged>.Invalid(
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

        TeamMembershipChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TeamMembershipChanged>.Success(changed);
    }
}