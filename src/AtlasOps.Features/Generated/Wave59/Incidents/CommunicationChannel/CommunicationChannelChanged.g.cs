namespace AtlasOps.Features.Incidents.CommunicationChannel;

public sealed record CommunicationChannelChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);