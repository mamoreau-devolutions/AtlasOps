namespace AtlasOps.Features.Connections.SshConnection;

public sealed record SshConnectionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);