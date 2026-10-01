namespace AtlasOps.Features.Connections.FileTransfer;

public sealed record FileTransferChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);