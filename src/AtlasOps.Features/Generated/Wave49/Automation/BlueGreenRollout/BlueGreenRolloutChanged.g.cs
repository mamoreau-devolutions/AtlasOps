namespace AtlasOps.Features.Automation.BlueGreenRollout;

public sealed record BlueGreenRolloutChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);