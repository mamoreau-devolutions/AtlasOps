namespace AtlasOps.Features.Governance.TeamMembership;

public sealed record UpdateTeamMembershipCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);