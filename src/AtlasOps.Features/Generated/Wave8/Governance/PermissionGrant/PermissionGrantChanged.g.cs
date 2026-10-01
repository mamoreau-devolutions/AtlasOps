namespace AtlasOps.Features.Governance.PermissionGrant;

public sealed record PermissionGrantChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);