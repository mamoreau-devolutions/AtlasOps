namespace AtlasOps.Features.Connections.TunnelDefinition;

public sealed record UpdateTunnelDefinitionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);