namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.WorkTracking.Contracts;
using AtlasOps.Integrations.WorkTracking.Core;

[TestClass]
public sealed class WorkTrackingServiceTests
{
    [TestMethod]
    public void ApplyTransition_StaleInvalidStateAndMissingFields_PreservesItemAndOrdersFields()
    {
        WorkTrackingService service = new();
        WorkItemReference item = Item("ABC-1", "Open", "7");
        WorkTransition transition = new(
            "Close",
            new HashSet<string>(["Ready"]),
            "Closed",
            new HashSet<string>(["zeta", "alpha"], StringComparer.OrdinalIgnoreCase));

        WorkTransitionResult result = service.ApplyTransition(
            item,
            transition,
            new Dictionary<string, string> { ["zeta"] = " " },
            "6");

        Assert.IsFalse(result.Succeeded);
        Assert.AreSame(item, result.Item);
        CollectionAssert.AreEqual(
            new[]
            {
                "The work item revision is stale.",
                "Transition 'Close' cannot start from state 'Open'.",
                "Required field 'alpha' is missing.",
                "Required field 'zeta' is missing.",
            },
            result.Diagnostics.ToArray());
    }

    [TestMethod]
    public void ApplyTransition_NumericAndTextRevisions_AdvanceWithCorrectFormat()
    {
        WorkTrackingService service = new();
        WorkTransition transition = new(
            "Start",
            new HashSet<string>(["Open"]),
            "Active",
            new HashSet<string>());

        WorkTransitionResult numeric = service.ApplyTransition(
            Item("A", "Open", "7"),
            transition,
            new Dictionary<string, string>(),
            "7");
        WorkTransitionResult text = service.ApplyTransition(
            Item("B", "Open", "etag"),
            transition,
            new Dictionary<string, string>(),
            "etag");

        Assert.IsTrue(numeric.Succeeded);
        Assert.AreEqual("Active", numeric.Item.State);
        Assert.AreEqual("8", numeric.Item.Revision);
        Assert.AreEqual("etag:1", text.Item.Revision);
        Assert.IsEmpty(text.Diagnostics);
    }

    [TestMethod]
    public void CreateDelta_UsesCaseInsensitiveKeysAndOrdersChangedAndDeleted()
    {
        WorkTrackingService service = new();

        WorkSynchronizationDelta result = service.CreateDelta(
            [Item("z", "Open", "1"), Item("A", "Open", "1"), Item("gone", "Open", "1")],
            [Item("a", "Open", "1"), Item("B", "Open", "1"), Item("z", "Open", "2")],
            "watermark");

        CollectionAssert.AreEqual(new[] { "B", "z" }, result.Changed.Select(static item => item.Key).ToArray());
        CollectionAssert.AreEqual(new[] { "gone" }, result.DeletedKeys.ToArray());
        Assert.AreEqual("watermark", result.Watermark);
    }

    private static WorkItemReference Item(string key, string state, string revision)
    {
        return new WorkItemReference("provider", "project", key, "title", state, revision, 1, []);
    }
}
