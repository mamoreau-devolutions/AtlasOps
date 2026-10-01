namespace AtlasOps.Features.Connections.RemoteClipboard;

public sealed record UpdateRemoteClipboardCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);