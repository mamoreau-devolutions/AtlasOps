namespace AtlasOps.Adapters.Tests;

using System.Text;

using AtlasOps.Adapters.Aws;
using AtlasOps.Adapters.Azure;
using AtlasOps.Adapters.Gcp;
using AtlasOps.Adapters.Persistence.Sqlite;
using AtlasOps.Scheduling;
using AtlasOps.Security;
using AtlasOps.Serialization;
using AtlasOps.Telemetry;
using AtlasOps.Tools.Migration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class PackageBackedIntegrationTests
{
    [TestMethod]
    public async Task SqliteOperationJournal_AppendAndRead_PreservesOrderedOperations()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"atlasops-{Guid.NewGuid():N}.db");
        try
        {
            SqliteOperationJournal journal = new(databasePath);
            await journal.InitializeAsync(CancellationToken.None);
            long first = await journal.AppendAsync(
                "inventory",
                "discover",
                "asset-01",
                new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero),
                """{"source":"test"}""",
                CancellationToken.None);
            long second = await journal.AppendAsync(
                "inventory",
                "reconcile",
                "asset-01",
                new DateTimeOffset(2026, 10, 1, 12, 1, 0, TimeSpan.Zero),
                """{"status":"matched"}""",
                CancellationToken.None);

            IReadOnlyList<SqliteOperationEntry> entries = await journal.ReadResourceAsync(
                "asset-01",
                10,
                CancellationToken.None);

            Assert.IsGreaterThan(0L, first);
            Assert.IsGreaterThan(first, second);
            Assert.HasCount(2, entries);
            Assert.AreEqual("reconcile", entries[0].Operation);
            Assert.AreEqual("discover", entries[1].Operation);
        }
        finally
        {
            File.Delete(databasePath);
            File.Delete(databasePath + "-shm");
            File.Delete(databasePath + "-wal");
        }
    }

    [TestMethod]
    public void ProviderCatalogs_DescribeDistinctTypedSdkIntegrations()
    {
        Assert.HasCount(6, AwsSdkCatalog.Services);
        Assert.HasCount(4, AzureSdkCatalog.Services);
        Assert.HasCount(2, GoogleSdkCatalog.Default.Scopes);
        Assert.HasCount(5, DatabaseProviderCatalog.Providers);
        Assert.AreEqual(
            AwsSdkCatalog.Services.Count,
            AwsSdkCatalog.Services.Select(static item => item.ClientType).Distinct().Count());
        Assert.AreEqual(
            DatabaseProviderCatalog.Providers.Count,
            DatabaseProviderCatalog.Providers.Select(static item => item.ClientType).Distinct().Count());
    }

    [TestMethod]
    public void SecurityCatalog_ContainsSecretAndHardwareBoundIntegrations()
    {
        Assert.HasCount(7, SecurityIntegrationCatalog.Integrations);
        Assert.IsTrue(SecurityIntegrationCatalog.Integrations.Any(static item => item.HandlesSecrets));
        Assert.IsTrue(SecurityIntegrationCatalog.Integrations.Any(static item => item.RequiresHardware));

        JwtInspectionResult result = new JwtInspector().Inspect("not-a-token", DateTimeOffset.UtcNow);

        Assert.IsFalse(result.Valid);
        Assert.IsNotEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void InterchangeService_SerializesOperationalDataAcrossFormats()
    {
        List<InterchangeRecord> records =
        [
            new() { Id = "asset-01", Category = "server", Value = 12.5m },
            new() { Id = "asset-02", Category = "network", Value = 7.5m },
        ];
        InterchangeService service = new();

        byte[] messagePack = service.SerializeMessagePack(records);
        byte[] protobuf = service.SerializeProtobuf(records);
        string yaml = service.SerializeYaml(records);
        string csv = service.SerializeCsv(records);
        string summary = service.RenderSummary("Total: {{ total }}", records);
        byte[] workbook = service.CreateWorkbook(records);
        byte[] archive = service.CreateArchive("records.csv", Encoding.UTF8.GetBytes(csv));

        Assert.IsNotEmpty(messagePack);
        Assert.IsNotEmpty(protobuf);
        Assert.Contains("asset-01", yaml);
        Assert.Contains("asset-02", csv);
        Assert.Contains("20", summary);
        Assert.IsNotEmpty(workbook);
        Assert.IsNotEmpty(archive);
        Assert.IsNotNull(service.ArchiveIntegrationType);
    }

    [TestMethod]
    public void CalendarScheduleService_ValidCron_ProducesNextOccurrenceAndCalendar()
    {
        CalendarScheduleResult result = new CalendarScheduleService().Create(
            "0 0 12 * * ?",
            "Daily inventory reconciliation",
            new DateTimeOffset(2026, 10, 1, 10, 0, 0, TimeSpan.Zero),
            TimeSpan.FromMinutes(30));

        Assert.IsTrue(result.Valid);
        Assert.IsNotNull(result.NextOccurrence);
        Assert.Contains("BEGIN:VCALENDAR", result.CalendarText);
        Assert.IsEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void StructuredEventCollector_RecordOperation_CapturesStructuredProperties()
    {
        using StructuredEventCollector collector = new();

        collector.RecordOperation("reconcile", "asset", TimeSpan.FromMilliseconds(42), true);

        StructuredEvent structuredEvent = Assert.ContainsSingle(collector.Events);
        Assert.AreEqual("Information", structuredEvent.Level);
        Assert.Contains("reconcile", structuredEvent.Message);
        Assert.Contains("Operation", structuredEvent.Properties.Keys);
        Assert.Contains("Succeeded", structuredEvent.Properties.Keys);
    }
}
