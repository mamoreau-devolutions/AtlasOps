namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.ServiceManagement.Contracts;
using AtlasOps.Integrations.ServiceManagement.Core;

[TestClass]
public sealed class ServiceLevelServiceTests
{
    private static readonly DateTimeOffset Start = new(2026, 7, 8, 9, 10, 11, TimeSpan.Zero);

    [TestMethod]
    public void Evaluate_SubtractsCompletedAndActivePausesAndCountsExactEscalationBoundary()
    {
        ServiceLevelService service = new();
        ServiceTicket ticket = Ticket("Open") with
        {
            AccumulatedPausedTime = TimeSpan.FromMinutes(10),
            PausedAt = Start.AddMinutes(50),
        };
        ServiceLevelPolicy policy = Policy([TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(30)]);

        ServiceLevelEvaluation result = service.Evaluate(ticket, policy, Start.AddHours(1));

        Assert.AreEqual(TimeSpan.FromMinutes(40), result.ActiveAge);
        Assert.AreEqual(TimeSpan.FromMinutes(20), result.Remaining);
        Assert.AreEqual(2, result.EscalationLevel);
        Assert.IsFalse(result.Breached);
    }

    [TestMethod]
    public void Evaluate_PriorityMismatchNegativeChronologyAndBreach_ReturnDiagnosticsAndClamp()
    {
        ServiceLevelService service = new();
        ServiceTicket future = Ticket("Open") with { CreatedAt = Start.AddHours(1), Priority = "P2" };
        ServiceLevelPolicy policy = Policy([TimeSpan.Zero]) with { Priority = "P1", ResolutionTarget = TimeSpan.Zero };

        ServiceLevelEvaluation result = service.Evaluate(future, policy, Start);

        Assert.AreEqual(TimeSpan.Zero, result.ActiveAge);
        Assert.AreEqual(TimeSpan.Zero, result.Remaining);
        Assert.AreEqual(1, result.EscalationLevel);
        Assert.IsTrue(result.Breached);
        CollectionAssert.AreEqual(
            new[]
            {
                "The service-level policy does not match the ticket priority.",
                "Ticket chronology is invalid.",
            },
            result.Diagnostics.ToArray());
    }

    [TestMethod]
    public void ApplyTransition_InvalidStateReturnsSameInstance()
    {
        ServiceLevelService service = new();
        ServiceTicket ticket = Ticket("Open");
        TicketTransition transition = new("Close", new HashSet<string>(["Resolved"]), "Closed", false, true);

        ServiceTicket result = service.ApplyTransition(ticket, transition, Start.AddMinutes(1), "2");

        Assert.AreSame(ticket, result);
    }

    [TestMethod]
    public void ApplyTransition_PauseResumeAndResolve_UpdateChronologyAndRevision()
    {
        ServiceLevelService service = new();
        ServiceTicket ticket = Ticket("Open");
        ServiceTicket paused = service.ApplyTransition(
            ticket,
            new TicketTransition("Pause", new HashSet<string>(["Open"]), "Waiting", true, false),
            Start.AddMinutes(5),
            "2");
        ServiceTicket resolved = service.ApplyTransition(
            paused,
            new TicketTransition("Resolve", new HashSet<string>(["Waiting"]), "Resolved", false, true),
            Start.AddMinutes(15),
            "3");

        Assert.AreEqual(Start.AddMinutes(5), paused.PausedAt);
        Assert.AreEqual("2", paused.Revision);
        Assert.IsNull(resolved.PausedAt);
        Assert.AreEqual(TimeSpan.FromMinutes(10), resolved.AccumulatedPausedTime);
        Assert.AreEqual(Start.AddMinutes(15), resolved.ResolvedAt);
        Assert.AreEqual("3", resolved.Revision);
    }

    [TestMethod]
    public void CreateEscalationSchedule_SortsThresholdsAndIncludesAccumulatedPause()
    {
        ServiceLevelService service = new();
        ServiceTicket ticket = Ticket("Open") with { AccumulatedPausedTime = TimeSpan.FromMinutes(5) };
        ServiceLevelPolicy policy = Policy([TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(10)]);

        IReadOnlyList<DateTimeOffset> result = service.CreateEscalationSchedule(ticket, policy);

        CollectionAssert.AreEqual(
            new[] { Start.AddMinutes(15), Start.AddMinutes(35) },
            result.ToArray());
    }

    private static ServiceTicket Ticket(string state)
    {
        return new ServiceTicket(
            "provider",
            "ticket",
            state,
            "P1",
            Start,
            Start,
            null,
            TimeSpan.Zero,
            null,
            "1");
    }

    private static ServiceLevelPolicy Policy(IReadOnlyList<TimeSpan> thresholds)
    {
        return new ServiceLevelPolicy("P1", TimeSpan.FromMinutes(10), TimeSpan.FromHours(1), thresholds);
    }
}
