namespace AtlasOps.Features.Connections.SshConnection;

public sealed record UpdateSshConnectionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);