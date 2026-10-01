namespace AtlasOps.Features.Platform.ExtensionMarketplace;

public sealed record ExtensionMarketplaceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);