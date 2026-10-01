namespace AtlasOps.Features.Governance.ActorIdentity;

public sealed record UpdateActorIdentityCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);