namespace AtlasOps.Features.Connections.ConnectionTemplate;

public sealed record UpdateConnectionTemplateCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);