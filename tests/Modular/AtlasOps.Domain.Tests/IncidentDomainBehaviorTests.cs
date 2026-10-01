namespace AtlasOps.Domain.Tests;

using AtlasOps.Modules.Incidents.Contracts;
using AtlasOps.Modules.Incidents.Core;

[TestClass]
public sealed class IncidentDomainBehaviorTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    [TestMethod]
    public void Validator_ExactBoundariesAcceptAndAdjacentValuesIdentifyTheOffendingField()
    {
        IncidentDetectionValidator validator = new();
        IncidentDetectionRecord boundary = Record() with
        {
            Name = new string('n', 160),
            Owner = new string('o', 120),
            Priority = 100,
            EstimatedCost = 0m,
            RiskScore = 1d,
            UpdatedAt = Now,
            DueAt = Now,
        };
        (string Field, IncidentDetectionRecord Record)[] invalidCases =
        [
            ("Name", boundary with { Name = new string('n', 161) }),
            ("Owner", boundary with { Owner = new string('o', 121) }),
            ("Priority", boundary with { Priority = 101 }),
            ("Priority", boundary with { Priority = -1 }),
            ("EstimatedCost", boundary with { EstimatedCost = -0.01m }),
            ("RiskScore", boundary with { RiskScore = 1.000001d }),
            ("RiskScore", boundary with { RiskScore = -0.000001d }),
            ("UpdatedAt", boundary with { UpdatedAt = Now.AddTicks(-1) }),
            ("DueAt", boundary with { DueAt = Now.AddTicks(-1) }),
        ];

        Assert.IsEmpty(validator.Validate(boundary));
        foreach ((string expectedField, IncidentDetectionRecord invalid) in invalidCases)
        {
            IReadOnlyList<IncidentDetectionValidationIssue> issues = validator.Validate(invalid);
            Assert.HasCount(1, issues, $"Expected exactly one issue for {expectedField}.");
            Assert.AreEqual(expectedField, issues[0].Field);
            Assert.IsFalse(string.IsNullOrWhiteSpace(issues[0].Code));
            Assert.IsFalse(string.IsNullOrWhiteSpace(issues[0].Message));
        }
    }

    [TestMethod]
    public void Policy_ExactRiskAndPriorityBoundaries_RequireOnlyTheirDocumentedOverrides()
    {
        IncidentDetectionPolicy policy = new();
        IncidentDetectionRecord boundary = Record(risk: 0.85d, priority: 89);
        IncidentDetectionCommand ordinary = Command(boundary);

        IncidentDetectionPolicyDecision allowed = policy.Evaluate(boundary, ordinary);
        IncidentDetectionPolicyDecision riskDenied = policy.Evaluate(Record(risk: 0.850001d, priority: 89), ordinary);
        IncidentDetectionRecord critical = Record(risk: 0.85d, priority: 90);
        IncidentDetectionPolicyDecision ticketDenied = policy.Evaluate(critical, Command(critical));
        IncidentDetectionPolicyDecision overridden = policy.Evaluate(
            Record(risk: 0.9d, priority: 100),
            Command(Record(risk: 0.9d, priority: 100), new Dictionary<string, string>
            {
                ["approval"] = "APR-1",
                ["changeTicket"] = "CHG-1",
            }));

        Assert.IsTrue(allowed.Allowed);
        Assert.AreEqual("approval-required", riskDenied.Code);
        Assert.AreEqual("ticket-required", ticketDenied.Code);
        Assert.IsTrue(overridden.Allowed);
        Assert.AreEqual("allowed", overridden.Code);
    }

    [TestMethod]
    public async Task Service_ValidTransition_PersistsIncrementedRecordAndPublishesEvent()
    {
        InMemoryIncidentDetectionRepository repository = new();
        BufferingIncidentDetectionEventSink sink = new();
        IncidentDetectionRecord original = Record();
        await repository.SaveAsync(original, 0, CancellationToken.None);
        IncidentDetectionService service = new(repository, sink);

        IncidentDetectionMutation result = await service.ExecuteAsync(
            Command(original, new Dictionary<string, string> { ["name"] = "Activated incident" }),
            CancellationToken.None);

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual("applied", result.Code);
        Assert.IsNotNull(result.Record);
        Assert.AreEqual(IncidentDetectionState.Active, result.Record.State);
        Assert.AreEqual("Activated incident", result.Record.Name);
        Assert.AreEqual(2L, result.Record.Revision);
        IncidentDetectionRecord? persisted = await repository.GetAsync(original.Id, CancellationToken.None);
        Assert.AreEqual(result.Record, persisted);
        Assert.HasCount(1, sink.Events);
        Assert.AreEqual("IncidentDetection.activate", sink.Events[0].EventType);
        Assert.AreEqual(2L, sink.Events[0].Revision);
    }

    [TestMethod]
    public async Task Service_RejectedTransition_LeavesRepositoryAndEventSinkUnchanged()
    {
        InMemoryIncidentDetectionRepository repository = new();
        BufferingIncidentDetectionEventSink sink = new();
        IncidentDetectionRecord original = Record(state: IncidentDetectionState.Archived);
        await repository.SaveAsync(original, 0, CancellationToken.None);
        IncidentDetectionService service = new(repository, sink);

        IncidentDetectionMutation result = await service.ExecuteAsync(Command(original), CancellationToken.None);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual("transition-denied", result.Code);
        Assert.AreEqual(original, await repository.GetAsync(original.Id, CancellationToken.None));
        Assert.IsEmpty(sink.Events);
    }

    [TestMethod]
    public async Task Service_MissingRecord_ReturnsNotFoundWithoutPublishing()
    {
        BufferingIncidentDetectionEventSink sink = new();
        IncidentDetectionService service = new(new InMemoryIncidentDetectionRepository(), sink);
        IncidentDetectionRecord absent = Record();

        IncidentDetectionMutation result = await service.ExecuteAsync(Command(absent), CancellationToken.None);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual("not-found", result.Code);
        Assert.IsNull(result.Record);
        Assert.IsNull(result.Event);
        Assert.IsEmpty(sink.Events);
    }

    [TestMethod]
    [DataRow(IncidentDetectionState.Draft, "activate", IncidentDetectionState.Active)]
    [DataRow(IncidentDetectionState.Draft, "archive", IncidentDetectionState.Archived)]
    [DataRow(IncidentDetectionState.Draft, "update", IncidentDetectionState.Draft)]
    [DataRow(IncidentDetectionState.Active, "pause", IncidentDetectionState.Paused)]
    [DataRow(IncidentDetectionState.Active, "complete", IncidentDetectionState.Completed)]
    [DataRow(IncidentDetectionState.Paused, "resume", IncidentDetectionState.Active)]
    [DataRow(IncidentDetectionState.Completed, "reopen", IncidentDetectionState.Active)]
    [DataRow(IncidentDetectionState.Archived, "restore", IncidentDetectionState.Active)]
    public void PolicyAndStateMachine_EachStateTransition_ProducesExpectedNextState(
        IncidentDetectionState source,
        string action,
        IncidentDetectionState expected)
    {
        IncidentDetectionRecord current = Record(state: source);
        IncidentDetectionCommand command = Command(current, action: action);
        IncidentDetectionPolicyDecision decision = new IncidentDetectionPolicy().Evaluate(current, command);
        IncidentDetectionMutation mutation = new IncidentDetectionStateMachine().Apply(current, command);

        Assert.IsTrue(decision.Allowed);
        Assert.AreEqual("allowed", decision.Code);
        Assert.IsTrue(mutation.Succeeded);
        Assert.IsNotNull(mutation.Record);
        Assert.AreEqual(expected, mutation.Record.State);
        Assert.AreEqual(current.Revision + 1, mutation.Record.Revision);
        Assert.IsNotNull(mutation.Event);
        Assert.AreEqual(source.ToString(), mutation.Event.Details["previousState"]);
        Assert.AreEqual(expected.ToString(), mutation.Event.Details["nextState"]);
        Assert.AreEqual(action, mutation.Event.Details["action"]);
    }

    [TestMethod]
    public async Task Repository_Query_CombinesFiltersAndOrdersByPriorityThenName()
    {
        InMemoryIncidentDetectionRepository repository = new();
        await repository.SaveAsync(Record(name: "Zulu", owner: "Alpha", priority: 80), 0, CancellationToken.None);
        await repository.SaveAsync(Record(id: Guid.NewGuid(), name: "Alpha", owner: "alpha", priority: 80), 0, CancellationToken.None);
        await repository.SaveAsync(Record(id: Guid.NewGuid(), name: "Ignored", owner: "Beta", priority: 100), 0, CancellationToken.None);

        IncidentDetectionPage page = await repository.QueryAsync(
            new(null, IncidentDetectionState.Draft, "ALPHA", 80, 0.5d, -20, 999),
            CancellationToken.None);

        Assert.AreEqual(0, page.Offset);
        Assert.AreEqual(500, page.Limit);
        Assert.AreEqual(2, page.TotalCount);
        CollectionAssert.AreEqual(new[] { "Alpha", "Zulu" }, page.Items.Select(static item => item.Name).ToArray());
    }

    [TestMethod]
    public async Task Repository_RevisionConflictsDeleteAndCancellation_PreserveStoredState()
    {
        InMemoryIncidentDetectionRepository repository = new();
        IncidentDetectionRecord original = Record();
        IncidentDetectionMutation created = await repository.SaveAsync(original, 0, CancellationToken.None);
        IncidentDetectionMutation conflict = await repository.SaveAsync(original with { Name = "Wrong" }, 99, CancellationToken.None);
        bool wrongDelete = await repository.DeleteAsync(original.Id, 99, CancellationToken.None);
        bool deleted = await repository.DeleteAsync(original.Id, original.Revision, CancellationToken.None);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        Assert.IsTrue(created.Succeeded);
        Assert.AreEqual("created", created.Code);
        Assert.IsFalse(conflict.Succeeded);
        Assert.AreEqual("revision-conflict", conflict.Code);
        Assert.AreEqual(original, conflict.Record);
        Assert.IsFalse(wrongDelete);
        Assert.IsTrue(deleted);
        Assert.IsNull(await repository.GetAsync(original.Id, CancellationToken.None));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            async () => await repository.GetAsync(original.Id, cancellation.Token));
    }

    [TestMethod]
    public void AnalyticsRankingAndHistory_PinDownOverdueScoringAndRestoreSemantics()
    {
        IncidentDetectionRecord overdue = Record(priority: 80, risk: 0.7d, dueAt: Now.AddMinutes(-1));
        IncidentDetectionRecord completed = Record(
            id: Guid.NewGuid(),
            name: "Completed",
            state: IncidentDetectionState.Completed,
            priority: 10,
            risk: 0.1d,
            dueAt: Now.AddDays(-1));
        IncidentDetectionAnalytics analytics = new IncidentDetectionAnalyticsEngine().Analyze([overdue, completed], Now);
        RankedIncidentDetection ranked = new IncidentDetectionRankingEngine().Rank([completed, overdue], Now, 1)[0];
        IncidentDetectionHistoryJournal history = new();
        history.Capture(overdue, Now, "before-change");
        IncidentDetectionRecord? restored = history.Restore(overdue.Id, overdue.Revision, Now.AddHours(1));

        Assert.AreEqual(2, analytics.TotalCount);
        Assert.AreEqual(1, analytics.OverdueCount);
        Assert.AreEqual(0.4d, analytics.AverageRisk, 0.0001d);
        Assert.AreEqual(overdue.Id, ranked.Record.Id);
        Assert.AreEqual(94d, ranked.Score);
        CollectionAssert.AreEquivalent(new[] { "high-priority", "elevated-risk", "overdue" }, ranked.Reasons.ToArray());
        Assert.IsNotNull(restored);
        Assert.AreEqual(overdue.Revision + 1, restored.Revision);
        Assert.AreEqual(Now.AddHours(1), restored.UpdatedAt);
        Assert.AreEqual(overdue.Name, restored.Name);
    }

    private static IncidentDetectionRecord Record(
        Guid? id = null,
        string name = "Incident",
        string owner = "Atlas",
        IncidentDetectionState state = IncidentDetectionState.Draft,
        int priority = 50,
        double risk = 0.5d,
        DateTimeOffset? dueAt = null) =>
        new(
            id ?? Guid.Parse("22222222-2222-2222-2222-222222222222"),
            name,
            owner,
            state,
            priority,
            10m,
            risk,
            Now,
            Now,
            dueAt ?? Now.AddDays(2),
            1,
            new Dictionary<string, string>());

    private static IncidentDetectionCommand Command(
        IncidentDetectionRecord record,
        IReadOnlyDictionary<string, string>? parameters = null,
        string action = "activate") =>
        new(record.Id, action, "operator", record.Revision, Now.AddMinutes(1), parameters ?? new Dictionary<string, string>());
}