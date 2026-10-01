namespace AtlasOps.Application.Tests;

using AtlasOps.Application;
using AtlasOps.Modules.Automation.Contracts;
using AtlasOps.Modules.Automation.Core;

[TestClass]
public sealed class ApplicationCompositionTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    [TestMethod]
    public void Registry_UsesCaseInsensitiveIdentityAndStableOrder()
    {
        ApplicationRegistry registry = new();
        Assert.IsTrue(registry.Register(new("z", "Zulu", "ops", 2, new Dictionary<string, string>())));
        Assert.IsTrue(registry.Register(new("a", "Alpha", "ops", 2, new Dictionary<string, string>())));
        Assert.IsTrue(registry.Register(new("first", "First", "core", 1, new Dictionary<string, string>())));

        Assert.IsFalse(registry.Register(new("Z", "Duplicate", "ops", 3, new Dictionary<string, string>())));
        CollectionAssert.AreEqual(new[] { "first", "a", "z" }, registry.Items.Select(static item => item.Id).ToArray());
        Assert.HasCount(2, registry.Find("OPS"));
        Assert.IsTrue(registry.TryGet("FIRST", out ApplicationDescriptor? descriptor));
        Assert.IsNotNull(descriptor);
        Assert.AreEqual("First", descriptor!.DisplayName);
    }

    [TestMethod]
    public async Task Pipeline_StopsAtFirstHandledStageAndReturnsConcreteUnhandledOutcome()
    {
        ApplicationPipeline pipeline = new();
        List<string> calls = new();
        pipeline.Add((_, _) =>
        {
            calls.Add("miss");
            return ValueTask.FromResult<ApplicationOutcome?>(null);
        });
        pipeline.Add((_, _) =>
        {
            calls.Add("handled");
            return ValueTask.FromResult<ApplicationOutcome?>(new(
                true,
                "accepted",
                "Handled.",
                TimeSpan.Zero,
                new Dictionary<string, string> { ["stage"] = "two" }));
        });
        pipeline.Add((_, _) =>
        {
            calls.Add("unexpected");
            return ValueTask.FromResult<ApplicationOutcome?>(null);
        });
        ApplicationOperation operation = new("op-1", "activate", "record", Now, new Dictionary<string, string>());

        ApplicationOutcome handled = await pipeline.ExecuteAsync(operation, CancellationToken.None);
        ApplicationOutcome unhandled = await new ApplicationPipeline().ExecuteAsync(operation, CancellationToken.None);

        Assert.IsTrue(handled.Succeeded);
        Assert.AreEqual("accepted", handled.Code);
        Assert.AreEqual("two", handled.Details["stage"]);
        CollectionAssert.AreEqual(new[] { "miss", "handled" }, calls);
        Assert.IsFalse(unhandled.Succeeded);
        Assert.AreEqual("unhandled", unhandled.Code);
        Assert.AreEqual("No pipeline stage handled the operation.", unhandled.Message);
    }

    [TestMethod]
    public async Task AutomationService_Activation_ComposesRepositoryPolicyStateMachineAndEventSink()
    {
        Guid id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        RunbookDesignRecord record = new(
            id,
            "Deploy",
            "Automation",
            RunbookDesignState.Draft,
            50,
            100m,
            0.5d,
            Now,
            Now,
            Now.AddDays(1),
            1,
            new Dictionary<string, string>());
        InMemoryRunbookDesignRepository repository = new();
        BufferingRunbookDesignEventSink events = new();
        await repository.SaveAsync(record, 0, CancellationToken.None);
        RunbookDesignService service = new(repository, events);

        RunbookDesignMutation mutation = await service.ExecuteAsync(
            new(id, "activate", "operator", 1, Now.AddMinutes(1), new Dictionary<string, string>()),
            CancellationToken.None);

        Assert.IsTrue(mutation.Succeeded);
        Assert.IsNotNull(mutation.Record);
        Assert.AreEqual(RunbookDesignState.Active, mutation.Record.State);
        Assert.AreEqual(2L, mutation.Record.Revision);
        Assert.HasCount(1, events.Events);
        Assert.AreEqual("RunbookDesign.activate", events.Events[0].EventType);
    }
}