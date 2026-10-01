namespace AtlasOps.Features.Automation.ManualGate;

public sealed record ManualGateChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);