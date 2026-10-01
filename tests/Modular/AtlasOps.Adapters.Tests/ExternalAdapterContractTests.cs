namespace AtlasOps.Adapters.Tests;

using System.Collections;
using System.Reflection;

[TestClass]
public sealed class ExternalAdapterContractTests
{
    [TestMethod]
    [DataRow("Aws")]
    [DataRow("Azure")]
    [DataRow("Gcp")]
    [DataRow("Iana")]
    [DataRow("EndOfLife")]
    [DataRow("Unicode")]
    [DataRow("NaturalEarth")]
    public async Task Adapter_AllPublicOperations_NormalizeValidatePageAndProbe(string adapterName)
    {
        string classPrefix = adapterName.Replace(".", string.Empty, StringComparison.Ordinal) + "Source";
        Assembly assembly = Assembly.Load($"AtlasOps.Adapters.{adapterName}");
        string ns = $"AtlasOps.Adapters.{adapterName}";
        Type requestType = RequiredType(assembly, $"{ns}.{classPrefix}Request");
        Type recordType = RequiredType(assembly, $"{ns}.{classPrefix}Record");
        Type normalizerType = RequiredType(assembly, $"{ns}.{classPrefix}Normalizer");
        Type policyType = RequiredType(assembly, $"{ns}.{classPrefix}Policy");
        Type transportType = RequiredType(assembly, $"{ns}.InMemory{classPrefix}Transport");
        Type adapterType = RequiredType(assembly, $"{ns}.{classPrefix}Adapter");
        DateTimeOffset observed = new(2026, 10, 1, 8, 0, 0, TimeSpan.FromHours(-4));

        dynamic normalizer = Activator.CreateInstance(normalizerType)!;
        dynamic normalized = normalizer.Normalize(
            " resource-1 ",
            " ",
            " ",
            -12.5m,
            observed,
            new Dictionary<string, string> { ["Owner"] = "Atlas" });
        Assert.AreEqual("RESOURCE-1", (string)normalized.Id);
        Assert.AreEqual("unknown", (string)normalized.Kind);
        Assert.AreEqual("global", (string)normalized.Region);
        Assert.AreEqual(0m, (decimal)normalized.Cost);
        Assert.AreEqual(observed.ToUniversalTime(), (DateTimeOffset)normalized.ObservedAt);
        Assert.AreEqual("c1b32631a2fa37991d6ffd1a0a1fea3faa3f794c48a08dea78a52bc3debf3f66", (string)normalized.SourceHash);
        Assert.AreEqual("Atlas", (string)normalized.Properties["owner"]);

        dynamic policy = Activator.CreateInstance(policyType)!;
        object invalidRequest = CreateRequest(requestType, " ", " ", null, 0);
        object invalidIssues = policy.Validate((dynamic)invalidRequest);
        CollectionAssert.AreEqual(
            new[] { "Operation is required.", "Resource is required.", "Page size must be between 1 and 1000." },
            ((IEnumerable)invalidIssues).Cast<string>().ToArray());
        Assert.AreEqual(0, ((IEnumerable)policy.Validate((dynamic)CreateRequest(requestType, "query", "vm", null, 1))).Cast<object>().Count());
        Assert.AreEqual(0, ((IEnumerable)policy.Validate((dynamic)CreateRequest(requestType, "query", "vm", null, 1000))).Cast<object>().Count());
        Assert.HasCount(1, ((IEnumerable)policy.Validate((dynamic)CreateRequest(requestType, "query", "vm", null, 1001))).Cast<object>().ToArray());

        object transport = Activator.CreateInstance(transportType)!;
        Array records = Array.CreateInstance(recordType, 3);
        records.SetValue(Normalize(normalizer, "b", "vm", "east", 2m, observed), 0);
        records.SetValue(Normalize(normalizer, "a", "vm", "west", 1m, observed), 1);
        records.SetValue(Normalize(normalizer, "c", "database", "west", 3m, observed), 2);
        transportType.GetMethod("Seed")!.Invoke(transport, [records]);
        dynamic adapter = Activator.CreateInstance(adapterType, transport)!;

        dynamic firstPage = await adapter.QueryAsync(
            (dynamic)CreateRequest(requestType, "query", "vm", null, 1),
            CancellationToken.None);
        Assert.HasCount(1, (IEnumerable<object>)firstPage.Items);
        Assert.AreEqual("A", (string)firstPage.Items[0].Id);
        Assert.AreEqual("1", (string)firstPage.ContinuationToken);
        Assert.IsTrue((bool)firstPage.HasMore);

        dynamic secondPage = await adapter.QueryAsync(
            (dynamic)CreateRequest(requestType, "query", "vm", "1", 10),
            CancellationToken.None);
        Assert.HasCount(1, (IEnumerable<object>)secondPage.Items);
        Assert.AreEqual("B", (string)secondPage.Items[0].Id);
        Assert.IsNull((string?)secondPage.ContinuationToken);
        Assert.IsFalse((bool)secondPage.HasMore);

        dynamic ready = await adapter.ProbeAsync(observed, CancellationToken.None);
        Assert.IsTrue((bool)ready.Healthy);
        Assert.AreEqual("ready", (string)ready.Code);
        Assert.AreEqual(observed, (DateTimeOffset)ready.CheckedAt);
        Assert.AreEqual(adapterName, (string)ready.Diagnostics["adapter"]);

        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        dynamic health = await adapter.ProbeAsync(observed, cancellation.Token);
        Assert.IsFalse((bool)health.Healthy);
        Assert.AreEqual("cancelled", (string)health.Code);
        Assert.AreEqual(observed, (DateTimeOffset)health.CheckedAt);
        Assert.AreEqual(0, ((IEnumerable)health.Diagnostics).Cast<object>().Count());
    }

    private static object Normalize(
        dynamic normalizer,
        string id,
        string kind,
        string region,
        decimal cost,
        DateTimeOffset observed) =>
        normalizer.Normalize(id, kind, region, cost, observed, new Dictionary<string, string>());

    private static object CreateRequest(Type requestType, string operation, string resource, string? token, int size) =>
        Activator.CreateInstance(
            requestType,
            operation,
            resource,
            token,
            size,
            new Dictionary<string, string> { ["tenant"] = "test" })!;

    private static Type RequiredType(Assembly assembly, string name) =>
        assembly.GetType(name) ?? throw new AssertFailedException($"Required public type {name} was not found.");
}