namespace AtlasOps.Features.Incidents.IncidentTemplate;

public sealed record UpdateIncidentTemplateCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);