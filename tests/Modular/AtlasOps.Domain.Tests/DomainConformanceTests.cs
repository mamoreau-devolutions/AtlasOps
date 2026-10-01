namespace AtlasOps.Domain.Tests;

using System.Collections;
using System.Reflection;

[TestClass]
public sealed class DomainConformanceTests
{
    [TestMethod]
    [DataRow("Workspaces", "PortfolioLifecycle")]
    [DataRow("Connections", "ProfileProvisioning")]
    [DataRow("Credentials", "CredentialLifecycle")]
    [DataRow("Inventory", "AssetInventory")]
    [DataRow("Automation", "RunbookDesign")]
    [DataRow("Deployments", "ReleasePlanning")]
    [DataRow("Incidents", "IncidentDetection")]
    [DataRow("Observability", "MetricCollection")]
    [DataRow("Governance", "PolicyAuthoring")]
    [DataRow("Identity", "IdentityProvisioning")]
    [DataRow("Documents", "DocumentAuthoring")]
    [DataRow("Geography", "CountryCatalog")]
    [DataRow("NetworkIntelligence", "ServiceCatalog")]
    [DataRow("Lifecycle", "ProductCatalog")]
    [DataRow("Localization", "LocaleCatalog")]
    [DataRow("CloudEconomics", "ProviderCatalog")]
    [DataRow("Geospatial", "LayerCatalog")]
    [DataRow("Compliance", "LicenseCatalog")]
    public void RepresentativeCapability_AllDomains_EnforcesValidationAndStateTransitions(
        string domain,
        string capability)
    {
        Assembly contracts = Assembly.Load($"AtlasOps.Modules.{domain}.Contracts");
        Assembly core = Assembly.Load($"AtlasOps.Modules.{domain}.Core");
        Type recordType = RequiredType(contracts, $"AtlasOps.Modules.{domain}.Contracts.{capability}Record");
        Type commandType = RequiredType(contracts, $"AtlasOps.Modules.{domain}.Contracts.{capability}Command");
        Type stateType = RequiredType(contracts, $"AtlasOps.Modules.{domain}.Contracts.{capability}State");
        Type validatorType = RequiredType(core, $"AtlasOps.Modules.{domain}.Core.{capability}Validator");
        Type machineType = RequiredType(core, $"AtlasOps.Modules.{domain}.Core.{capability}StateMachine");
        DateTimeOffset now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

        object valid = CreateRecord(recordType, stateType, now);
        object validator = Activator.CreateInstance(validatorType)!;
        object validIssues = validatorType.GetMethod("Validate")!.Invoke(validator, [valid])!;
        Assert.AreEqual(0, ((IEnumerable)validIssues).Cast<object>().Count(), $"{domain} rejected a valid record.");

        object invalid = CreateRecord(recordType, stateType, now, invalid: true);
        object invalidIssues = validatorType.GetMethod("Validate")!.Invoke(validator, [invalid])!;
        object[] issues = ((IEnumerable)invalidIssues).Cast<object>().ToArray();
        Assert.HasCount(8, issues, $"{domain} did not report every independent invalid field.");
        CollectionAssert.AreEquivalent(
            new[] { "Id", "Name", "Owner", "Priority", "EstimatedCost", "RiskScore", "UpdatedAt", "DueAt" },
            issues.Select(static issue => (string)issue.GetType().GetProperty("Field")!.GetValue(issue)!).ToArray());

        object command = Activator.CreateInstance(
            commandType,
            ((dynamic)valid).Id,
            "activate",
            "operator",
            1L,
            now.AddMinutes(1),
            new Dictionary<string, string>())!;
        object mutation = machineType.GetMethod("Apply")!.Invoke(Activator.CreateInstance(machineType), [valid, command])!;
        Assert.IsTrue((bool)mutation.GetType().GetProperty("Succeeded")!.GetValue(mutation)!);
        Assert.AreEqual("applied", mutation.GetType().GetProperty("Code")!.GetValue(mutation));
        object updated = mutation.GetType().GetProperty("Record")!.GetValue(mutation)!;
        Assert.AreEqual("Active", updated.GetType().GetProperty("State")!.GetValue(updated)!.ToString());
        Assert.AreEqual(2L, updated.GetType().GetProperty("Revision")!.GetValue(updated));

        object unknownCommand = Activator.CreateInstance(
            commandType,
            ((dynamic)valid).Id,
            "unsupported",
            "operator",
            1L,
            now,
            new Dictionary<string, string>())!;
        object rejected = machineType.GetMethod("Apply")!.Invoke(Activator.CreateInstance(machineType), [valid, unknownCommand])!;
        Assert.IsFalse((bool)rejected.GetType().GetProperty("Succeeded")!.GetValue(rejected)!);
        Assert.AreEqual("unknown-action", rejected.GetType().GetProperty("Code")!.GetValue(rejected));
        Assert.AreSame(valid, rejected.GetType().GetProperty("Record")!.GetValue(rejected));
    }

    private static object CreateRecord(Type recordType, Type stateType, DateTimeOffset now, bool invalid = false)
    {
        object state = Enum.Parse(stateType, "Draft");
        return Activator.CreateInstance(
            recordType,
            invalid ? Guid.Empty : Guid.Parse("11111111-1111-1111-1111-111111111111"),
            invalid ? string.Empty : "Representative record",
            invalid ? " " : "atlas-team",
            state,
            invalid ? -1 : 50,
            invalid ? -0.01m : 125m,
            invalid ? 1.01d : 0.5d,
            now,
            invalid ? now.AddMinutes(-1) : now,
            invalid ? now.AddDays(-1) : now.AddDays(1),
            1L,
            new Dictionary<string, string> { ["environment"] = "test" })!;
    }

    private static Type RequiredType(Assembly assembly, string name) =>
        assembly.GetType(name) ?? throw new AssertFailedException($"Required public type {name} was not found.");
}