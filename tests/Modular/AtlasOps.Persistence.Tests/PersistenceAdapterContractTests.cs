namespace AtlasOps.Persistence.Tests;

using System.Collections;
using System.Reflection;

[TestClass]
public sealed class PersistenceAdapterContractTests
{
    [TestMethod]
    [DataRow("Persistence.Local")]
    [DataRow("Persistence.Sqlite")]
    [DataRow("Persistence.Turso")]
    public async Task PersistenceAdapter_ValidAndInvalidRequests_PreservePagingAndCancellationContracts(string adapterName)
    {
        string classPrefix = adapterName.Replace(".", string.Empty, StringComparison.Ordinal) + "Source";
        Assembly assembly = Assembly.Load($"AtlasOps.Adapters.{adapterName}");
        string ns = $"AtlasOps.Adapters.{adapterName}";
        Type requestType = RequiredType(assembly, $"{ns}.{classPrefix}Request");
        Type recordType = RequiredType(assembly, $"{ns}.{classPrefix}Record");
        Type normalizerType = RequiredType(assembly, $"{ns}.{classPrefix}Normalizer");
        Type transportType = RequiredType(assembly, $"{ns}.InMemory{classPrefix}Transport");
        Type adapterType = RequiredType(assembly, $"{ns}.{classPrefix}Adapter");
        DateTimeOffset observed = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);
        dynamic normalizer = Activator.CreateInstance(normalizerType)!;
        object transport = Activator.CreateInstance(transportType)!;
        Array records = Array.CreateInstance(recordType, 2);
        records.SetValue(normalizer.Normalize("two", "snapshot", "local", 2m, observed, new Dictionary<string, string>()), 0);
        records.SetValue(normalizer.Normalize("one", "snapshot", "local", 1m, observed, new Dictionary<string, string>()), 1);
        transportType.GetMethod("Seed")!.Invoke(transport, [records]);
        dynamic adapter = Activator.CreateInstance(adapterType, transport)!;

        dynamic invalid = await adapter.QueryAsync(
            (dynamic)CreateRequest(requestType, string.Empty, string.Empty, null, 0),
            CancellationToken.None);
        Assert.IsEmpty((IEnumerable<object>)invalid.Items);
        Assert.IsNull((string?)invalid.ContinuationToken);
        Assert.IsFalse((bool)invalid.HasMore);

        dynamic first = await adapter.QueryAsync(
            (dynamic)CreateRequest(requestType, "read", "snapshot", null, 1),
            CancellationToken.None);
        Assert.HasCount(1, (IEnumerable<object>)first.Items);
        Assert.AreEqual("ONE", (string)first.Items[0].Id);
        Assert.AreEqual("1", (string)first.ContinuationToken);
        Assert.IsTrue((bool)first.HasMore);

        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        dynamic health = await adapter.ProbeAsync(observed, cancellation.Token);
        Assert.AreEqual("cancelled", (string)health.Code);
        Assert.IsFalse((bool)health.Healthy);
    }

    private static object CreateRequest(Type requestType, string operation, string resource, string? token, int size) =>
        Activator.CreateInstance(requestType, operation, resource, token, size, new Dictionary<string, string>())!;

    private static Type RequiredType(Assembly assembly, string name) =>
        assembly.GetType(name) ?? throw new AssertFailedException($"Required public type {name} was not found.");
}