namespace AtlasOps.Features.Connections.TunnelDefinition;

public sealed record TunnelDefinitionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);