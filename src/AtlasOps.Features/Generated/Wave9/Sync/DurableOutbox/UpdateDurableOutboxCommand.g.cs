namespace AtlasOps.Features.Sync.DurableOutbox;

public sealed record UpdateDurableOutboxCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);