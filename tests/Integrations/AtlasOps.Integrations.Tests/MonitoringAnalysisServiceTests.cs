namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.Monitoring.Contracts;
using AtlasOps.Integrations.Monitoring.Core;

[TestClass]
public sealed class MonitoringAnalysisServiceTests
{
    private static readonly DateTimeOffset Start = new(2026, 6, 7, 8, 9, 10, TimeSpan.Zero);

    [TestMethod]
    public void CreateWindow_UsesInclusiveStartExclusiveEndAndComputesStatistics()
    {
        MonitoringAnalysisService service = new();
        MetricSample[] samples =
        [
            Sample(Start.AddTicks(-1), 100),
            Sample(Start, 2),
            Sample(Start.AddMinutes(1), 6),
            Sample(Start.AddMinutes(2), 4),
            Sample(Start.AddMinutes(3), 200),
        ];

        MetricWindow result = service.CreateWindow(samples, Start, Start.AddMinutes(3));

        Assert.AreEqual(2d, result.Minimum);
        Assert.AreEqual(6d, result.Maximum);
        Assert.AreEqual(4d, result.Average);
        Assert.AreEqual(3, result.SampleCount);
    }

    [TestMethod]
    public void CreateWindow_InvalidOrEmptyWindow_ReturnsZeroStatistics()
    {
        MonitoringAnalysisService service = new();

        MetricWindow invalid = service.CreateWindow([], Start, Start);
        MetricWindow empty = service.CreateWindow([], Start, Start.AddMinutes(1));

        Assert.AreEqual(0, invalid.SampleCount);
        Assert.AreEqual(0d, invalid.Average);
        Assert.AreEqual(0, empty.SampleCount);
        Assert.AreEqual(Start.AddMinutes(1), empty.End);
    }

    [TestMethod]
    public void Correlate_WindowBoundaryStaysTogetherBeyondBoundarySplitsAndOrdersNewestFirst()
    {
        MonitoringAnalysisService service = new();
        AlertSignal first = Alert("resource", "1", "warning", Start, Start.AddMinutes(1));
        AlertSignal atBoundary = Alert("resource", "2", "critical", Start.AddMinutes(5), null);
        AlertSignal beyond = Alert("resource", "3", "error", Start.AddMinutes(5).AddTicks(1), Start.AddMinutes(6));

        IReadOnlyList<AlertCorrelationGroup> result = service.Correlate(
            [beyond, atBoundary, first],
            TimeSpan.FromMinutes(5));

        Assert.HasCount(2, result);
        Assert.AreEqual("3", result[0].Alerts.Single().AlertId);
        Assert.AreEqual("critical", result[1].HighestSeverity);
        Assert.IsNull(result[1].ResolvedAt);
        CollectionAssert.AreEqual(new[] { "1", "2" }, result[1].Alerts.Select(static alert => alert.AlertId).ToArray());
    }

    [TestMethod]
    public void Correlate_AllResolvedGroupUsesLatestResolutionAndSeparatesResources()
    {
        MonitoringAnalysisService service = new();
        AlertSignal a = Alert("a", "1", "information", Start, Start.AddMinutes(1));
        AlertSignal b = Alert("a", "2", "unknown", Start.AddSeconds(1), Start.AddMinutes(2));
        AlertSignal c = Alert("b", "3", "error", Start, Start.AddMinutes(3));

        IReadOnlyList<AlertCorrelationGroup> result = service.Correlate([a, b, c], TimeSpan.FromMinutes(1));

        Assert.HasCount(2, result);
        AlertCorrelationGroup groupA = result.Single(static group => group.ResourceId == "a");
        Assert.AreEqual(Start.AddMinutes(2), groupA.ResolvedAt);
        Assert.AreEqual("information", groupA.HighestSeverity);
    }

    private static MetricSample Sample(DateTimeOffset timestamp, double value)
    {
        return new MetricSample("source", "metric", timestamp, value, new Dictionary<string, string>());
    }

    private static AlertSignal Alert(
        string resource,
        string id,
        string severity,
        DateTimeOffset startedAt,
        DateTimeOffset? resolvedAt)
    {
        return new AlertSignal(
            "source",
            id,
            resource,
            severity,
            "summary",
            startedAt,
            resolvedAt,
            new Dictionary<string, string>());
    }
}
