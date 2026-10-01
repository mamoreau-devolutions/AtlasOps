namespace AtlasOps.Features.Connections.FileTransfer;

public sealed record UpdateFileTransferCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);