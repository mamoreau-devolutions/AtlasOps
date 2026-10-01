namespace AtlasOps.Features.Incidents.CommunicationChannel;

public sealed record UpdateCommunicationChannelCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);