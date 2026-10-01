namespace AtlasOps.Features.Connections.KubernetesContext;

public sealed record UpdateKubernetesContextCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);