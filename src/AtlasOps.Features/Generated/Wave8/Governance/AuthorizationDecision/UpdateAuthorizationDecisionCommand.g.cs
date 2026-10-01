namespace AtlasOps.Features.Governance.AuthorizationDecision;

public sealed record UpdateAuthorizationDecisionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);