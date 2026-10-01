namespace AtlasOps.Features.Connections.KubernetesContext;

public sealed record KubernetesContextChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);