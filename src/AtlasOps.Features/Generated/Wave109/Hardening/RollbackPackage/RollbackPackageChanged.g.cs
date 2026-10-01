namespace AtlasOps.Features.Hardening.RollbackPackage;

public sealed record RollbackPackageChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);