namespace AtlasOps.Features.Platform.ExtensionMarketplace;

public sealed record UpdateExtensionMarketplaceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);