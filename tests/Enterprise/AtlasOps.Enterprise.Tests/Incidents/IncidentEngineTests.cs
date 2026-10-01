namespace AtlasOps.Enterprise.Tests.Incidents;

using AtlasOps.Enterprise.Contracts.Incidents;
using AtlasOps.Enterprise.Core.Incidents;
using AtlasOps.Enterprise.Core.Scenarios;

[TestClass]
public sealed class IncidentEngineTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-10-01T12:00:00Z");

    [TestMethod]
    public void Transition_NewToResolved_RejectsInvalidStateChange()
    {
        Incident incident = EnterpriseScenarioCatalog.CreateIncidents(Now)[0] with
        {
            State = IncidentState.New,
            UpdatedAt = Now,
        };
        IncidentLifecycleService service = new();

        IncidentTransitionResult result = service.Transition(incident, IncidentState.Resolved, "operator", Now);

        Assert.IsFalse(result.Succeeded);
        Assert.AreSame(incident, result.Incident);
        Assert.IsNull(result.TimelineEntry);
    }

    [TestMethod]
    public void Transition_TriagedToAcknowledged_SetsAcknowledgementAndTimeline()
    {
        Incident incident = EnterpriseScenarioCatalog.CreateIncidents(Now)[0] with
        {
            State = IncidentState.Triaged,
            AcknowledgedAt = null,
        };
        IncidentLifecycleService service = new();

        IncidentTransitionResult result = service.Transition(incident, IncidentState.Acknowledged, "operator", Now, "Responder accepted page");

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(Now, result.Incident.AcknowledgedAt);
        Assert.IsNotNull(result.TimelineEntry);
        Assert.AreEqual(IncidentTimelineKind.StateChanged, result.TimelineEntry.Kind);
    }

    [TestMethod]
    public void Calculate_Sev1PastDeadline_ReportsResolutionBreach()
    {
        Incident incident = new(
            "INC-X",
            "Critical outage",
            "Service unavailable",
            IncidentSeverity.Sev1,
            IncidentState.Investigating,
            "api",
            "owner",
            Now.AddHours(-2),
            Now,
            Now.AddHours(-1.9));
        IncidentSlaService service = new(IncidentSlaService.CreateDefaultTargets());

        IncidentSlaStatus result = service.Calculate(incident, Now);

        Assert.IsTrue(result.IsResolutionBreached);
        Assert.IsGreaterThan(100d, result.ResolutionBudgetConsumedPercent);
    }

    [TestMethod]
    public void FindImpactedServices_CheckoutFailure_ReturnsTransitiveDependents()
    {
        ServiceImpactAnalyzer analyzer = new();

        IReadOnlyList<ServiceImpact> result = analyzer.FindImpactedServices(
            "payment-gateway",
            EnterpriseScenarioCatalog.CreateServiceDependencies());

        Assert.HasCount(3, result);
        Assert.IsTrue(result.Any(static impact => impact.ServiceId == "checkout-api" && impact.Distance == 1));
        Assert.IsTrue(result.Any(static impact => impact.ServiceId == "web-store" && impact.Distance == 2));
    }
}
