namespace AtlasOps.Features.Editor.SyntaxProfile;

public sealed record SyntaxProfileChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);