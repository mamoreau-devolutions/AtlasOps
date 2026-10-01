namespace AtlasOps.Features.Governance.AuditEnvelope;

public sealed record UpdateAuditEnvelopeCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);