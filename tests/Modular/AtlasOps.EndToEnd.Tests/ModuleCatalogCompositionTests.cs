namespace AtlasOps.EndToEnd.Tests;

using AtlasOps.Composition;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;

[TestClass]
public sealed class ModuleCatalogCompositionTests
{
    private static readonly IReadOnlyDictionary<string, (string View, string ViewModel, string Namespace)> EnterpriseWorkbenches =
        new Dictionary<string, (string View, string ViewModel, string Namespace)>(StringComparer.Ordinal)
        {
            ["Inventory"] = ("InventoryStudioView", "InventoryStudioViewModel", "AtlasOps.Enterprise.Avalonia.Inventory"),
            ["Automation"] = ("WorkflowStudioView", "WorkflowStudioViewModel", "AtlasOps.Enterprise.Avalonia.Workflow"),
            ["Incidents"] = ("IncidentStudioView", "IncidentStudioViewModel", "AtlasOps.Enterprise.Avalonia.Incidents"),
            ["Observability"] = ("ReportingStudioView", "ReportingStudioViewModel", "AtlasOps.Enterprise.Avalonia.Reporting"),
            ["Governance"] = ("PolicyStudioView", "PolicyStudioViewModel", "AtlasOps.Enterprise.Avalonia.Policy"),
        };

    private static readonly string[] ExpectedIds =
    [
        "Workspaces",
        "Connections",
        "Credentials",
        "Inventory",
        "Automation",
        "Deployments",
        "Incidents",
        "Observability",
        "Governance",
        "Identity",
        "Documents",
        "Geography",
        "NetworkIntelligence",
        "Lifecycle",
        "Localization",
        "CloudEconomics",
        "Geospatial",
        "Compliance",
    ];

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        AppBuilder.Configure<Application>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();
    }

    [TestMethod]
    public void Catalog_Create_ReturnsCompleteUniqueDeterministicModuleComposition()
    {
        IReadOnlyList<AtlasOpsModuleDescriptor> first = AtlasOpsModuleCatalog.Create();
        IReadOnlyList<AtlasOpsModuleDescriptor> second = AtlasOpsModuleCatalog.Create();

        Assert.HasCount(18, first);
        CollectionAssert.AreEqual(ExpectedIds, first.Select(static descriptor => descriptor.Id).ToArray());
        CollectionAssert.AreEqual(ExpectedIds, second.Select(static descriptor => descriptor.Id).ToArray());
        Assert.AreEqual(18, first.Select(static descriptor => descriptor.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.IsTrue(first.All(static descriptor => descriptor.CapabilityCount == 30));
        Assert.IsTrue(first.All(static descriptor => !string.IsNullOrWhiteSpace(descriptor.DisplayName)));
        Assert.AreNotSame(first, second);
        Assert.AreNotSame(first[0], second[0]);
    }

    [TestMethod]
    public void Catalog_Factories_CreateExpectedWorkbenchTypes()
    {
        IReadOnlyList<AtlasOpsModuleDescriptor> catalog = AtlasOpsModuleCatalog.Create();

        foreach (AtlasOpsModuleDescriptor descriptor in catalog)
        {
            UserControl view = descriptor.CreateView();
            bool isEnterpriseWorkbench = EnterpriseWorkbenches.TryGetValue(
                descriptor.Id,
                out (string View, string ViewModel, string Namespace) expected);
            string expectedView = isEnterpriseWorkbench
                ? expected.View
                : $"{descriptor.Id}WorkbenchView";
            string expectedViewModel = isEnterpriseWorkbench
                ? expected.ViewModel
                : $"{descriptor.Id}WorkbenchViewModel";
            string expectedNamespace = isEnterpriseWorkbench
                ? expected.Namespace
                : $"AtlasOps.Modules.{descriptor.Id}.Avalonia";

            Assert.AreEqual(
                expectedView,
                view.GetType().Name,
                $"Factory for {descriptor.Id} returned the wrong workbench.");
            Assert.AreEqual(
                expectedNamespace,
                view.GetType().Namespace,
                $"Factory for {descriptor.Id} returned a workbench from the wrong module.");
            Assert.IsNotNull(view.DataContext);
            Assert.AreEqual(
                expectedViewModel,
                view.DataContext.GetType().Name,
                $"Factory for {descriptor.Id} assigned the wrong view model.");
        }
    }
}