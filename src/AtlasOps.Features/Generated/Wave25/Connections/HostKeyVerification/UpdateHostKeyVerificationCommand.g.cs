namespace AtlasOps.Features.Connections.HostKeyVerification;

public sealed record UpdateHostKeyVerificationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);