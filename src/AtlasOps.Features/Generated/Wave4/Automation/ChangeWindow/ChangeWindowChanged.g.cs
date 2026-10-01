namespace AtlasOps.Features.Automation.ChangeWindow;

public sealed record ChangeWindowChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);