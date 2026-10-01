namespace AtlasOps.Features.Incidents.EscalationPolicy;

public sealed record UpdateEscalationPolicyCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);