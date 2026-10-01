namespace AtlasOps.Features.Incidents.AlertEnrichment;

public sealed record UpdateAlertEnrichmentCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);