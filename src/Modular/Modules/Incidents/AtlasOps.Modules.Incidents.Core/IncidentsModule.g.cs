namespace AtlasOps.Modules.Incidents.Core;

using System.Collections.Generic;

public sealed record IncidentsCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class IncidentsModule
{
    public const string Id = "Incidents";
    public const string DisplayName = "Incidents and alerting";
    public static IReadOnlyList<IncidentsCapabilityDescriptor> Capabilities { get; } = new IncidentsCapabilityDescriptor[]
    {
        new("Incidents.IncidentDetection", "Incident detection", "Incident", "Detection", "Coordinates incidents and alerting for Incident detection."),
        new("Incidents.IncidentTriage", "Incident triage", "Incident", "Triage", "Coordinates incidents and alerting for Incident triage."),
        new("Incidents.IncidentAssignment", "Incident assignment", "Incident", "Assignment", "Coordinates incidents and alerting for Incident assignment."),
        new("Incidents.IncidentEscalation", "Incident escalation", "Incident", "Escalation", "Coordinates incidents and alerting for Incident escalation."),
        new("Incidents.IncidentResolution", "Incident resolution", "Incident", "Resolution", "Coordinates incidents and alerting for Incident resolution."),
        new("Incidents.IncidentReview", "Incident review", "Incident", "Review", "Coordinates incidents and alerting for Incident review."),
        new("Incidents.AlertDetection", "Alert detection", "Alert", "Detection", "Coordinates incidents and alerting for Alert detection."),
        new("Incidents.AlertTriage", "Alert triage", "Alert", "Triage", "Coordinates incidents and alerting for Alert triage."),
        new("Incidents.AlertAssignment", "Alert assignment", "Alert", "Assignment", "Coordinates incidents and alerting for Alert assignment."),
        new("Incidents.AlertEscalation", "Alert escalation", "Alert", "Escalation", "Coordinates incidents and alerting for Alert escalation."),
        new("Incidents.AlertResolution", "Alert resolution", "Alert", "Resolution", "Coordinates incidents and alerting for Alert resolution."),
        new("Incidents.AlertReview", "Alert review", "Alert", "Review", "Coordinates incidents and alerting for Alert review."),
        new("Incidents.EscalationDetection", "Escalation detection", "Escalation", "Detection", "Coordinates incidents and alerting for Escalation detection."),
        new("Incidents.EscalationTriage", "Escalation triage", "Escalation", "Triage", "Coordinates incidents and alerting for Escalation triage."),
        new("Incidents.EscalationAssignment", "Escalation assignment", "Escalation", "Assignment", "Coordinates incidents and alerting for Escalation assignment."),
        new("Incidents.EscalationEscalation", "Escalation escalation", "Escalation", "Escalation", "Coordinates incidents and alerting for Escalation escalation."),
        new("Incidents.EscalationResolution", "Escalation resolution", "Escalation", "Resolution", "Coordinates incidents and alerting for Escalation resolution."),
        new("Incidents.EscalationReview", "Escalation review", "Escalation", "Review", "Coordinates incidents and alerting for Escalation review."),
        new("Incidents.TimelineDetection", "Timeline detection", "Timeline", "Detection", "Coordinates incidents and alerting for Timeline detection."),
        new("Incidents.TimelineTriage", "Timeline triage", "Timeline", "Triage", "Coordinates incidents and alerting for Timeline triage."),
        new("Incidents.TimelineAssignment", "Timeline assignment", "Timeline", "Assignment", "Coordinates incidents and alerting for Timeline assignment."),
        new("Incidents.TimelineEscalation", "Timeline escalation", "Timeline", "Escalation", "Coordinates incidents and alerting for Timeline escalation."),
        new("Incidents.TimelineResolution", "Timeline resolution", "Timeline", "Resolution", "Coordinates incidents and alerting for Timeline resolution."),
        new("Incidents.TimelineReview", "Timeline review", "Timeline", "Review", "Coordinates incidents and alerting for Timeline review."),
        new("Incidents.RunbookDetection", "Runbook detection", "Runbook", "Detection", "Coordinates incidents and alerting for Runbook detection."),
        new("Incidents.RunbookTriage", "Runbook triage", "Runbook", "Triage", "Coordinates incidents and alerting for Runbook triage."),
        new("Incidents.RunbookAssignment", "Runbook assignment", "Runbook", "Assignment", "Coordinates incidents and alerting for Runbook assignment."),
        new("Incidents.RunbookEscalation", "Runbook escalation", "Runbook", "Escalation", "Coordinates incidents and alerting for Runbook escalation."),
        new("Incidents.RunbookResolution", "Runbook resolution", "Runbook", "Resolution", "Coordinates incidents and alerting for Runbook resolution."),
        new("Incidents.RunbookReview", "Runbook review", "Runbook", "Review", "Coordinates incidents and alerting for Runbook review."),
    };
}