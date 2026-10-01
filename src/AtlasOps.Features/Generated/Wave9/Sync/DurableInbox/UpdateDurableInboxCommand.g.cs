namespace AtlasOps.Features.Sync.DurableInbox;

public sealed record UpdateDurableInboxCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);