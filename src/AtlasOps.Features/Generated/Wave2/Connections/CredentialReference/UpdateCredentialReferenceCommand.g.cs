namespace AtlasOps.Features.Connections.CredentialReference;

public sealed record UpdateCredentialReferenceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);