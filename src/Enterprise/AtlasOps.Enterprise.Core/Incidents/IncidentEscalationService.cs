namespace AtlasOps.Enterprise.Core.Incidents;

using AtlasOps.Enterprise.Contracts.Incidents;

public sealed class IncidentEscalationService
{
    private readonly IncidentSlaService slaService;

    public IncidentEscalationService(IncidentSlaService slaService)
    {
        this.slaService = slaService;
    }

    public EscalationRecommendation Evaluate(
        Incident incident,
        IReadOnlyList<OnCallShift> shifts,
        DateTimeOffset now)
    {
        IncidentSlaStatus sla = this.slaService.Calculate(incident, now);
        ServiceLevelTarget target = this.slaService.GetTarget(incident.Severity);
        OnCallShift? activeShift = shifts
            .Where(shift => shift.StartsAt <= now && shift.EndsAt > now)
            .OrderBy(static shift => shift.StartsAt)
            .FirstOrDefault();

        int level = 0;
        string reason = "No escalation is required.";
        if (sla.IsResolutionBreached)
        {
            level = 3;
            reason = "The resolution target is breached.";
        }
        else if (sla.IsAcknowledgementBreached)
        {
            level = 2;
            reason = "The acknowledgement target is breached.";
        }
        else if (sla.ResolutionRemaining <= target.EscalationLeadTime)
        {
            level = 1;
            reason = "The incident is within the resolution escalation window.";
        }
        else if (incident.Severity == IncidentSeverity.Sev1 && incident.State < IncidentState.Investigating)
        {
            level = 1;
            reason = "A Sev1 incident has not reached investigation.";
        }

        string targetName = level switch
        {
            3 => activeShift?.Secondary ?? "executive incident sponsor",
            2 => activeShift?.Secondary ?? "incident manager",
            1 => activeShift?.Primary ?? "on-call responder",
            _ => activeShift?.Primary ?? "assigned owner",
        };
        return new(level, level > 0, targetName, reason, now);
    }
}
