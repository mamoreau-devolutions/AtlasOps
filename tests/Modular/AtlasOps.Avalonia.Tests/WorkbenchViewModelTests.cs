namespace AtlasOps.Avalonia.Tests;

using AtlasOps.Modules.CloudEconomics.Avalonia;
using AtlasOps.Modules.Workspaces.Avalonia;

[TestClass]
public sealed class WorkbenchViewModelTests
{
    [TestMethod]
    public void WorkspacesSearch_FiltersByAreaAndRaisesOnlyContractedNotifications()
    {
        WorkspacesWorkbenchViewModel viewModel = new();
        List<string?> notifications = new();
        viewModel.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);

        viewModel.SearchText = "Portfolio";

        Assert.AreEqual("Workspaces and projects", viewModel.Title);
        Assert.AreEqual("30 operational capabilities across five business areas.", viewModel.Summary);
        Assert.HasCount(6, viewModel.Capabilities);
        Assert.IsTrue(viewModel.Capabilities.All(static capability => capability.Area == "Portfolio"));
        CollectionAssert.AreEqual(new[] { "SearchText", "Capabilities" }, notifications);

        viewModel.SearchText = "Portfolio";
        Assert.HasCount(2, notifications);
    }

    [TestMethod]
    public void CloudEconomicsSearch_NullResetsResultsAndSelectionRaisesOnce()
    {
        CloudEconomicsWorkbenchViewModel viewModel = new();
        List<string?> notifications = new();
        viewModel.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);
        viewModel.SearchText = "Forecasting";
        Assert.HasCount(5, viewModel.Capabilities);

        viewModel.SelectedCapability = viewModel.Capabilities[0];
        viewModel.SelectedCapability = viewModel.Capabilities[0];
        viewModel.SearchText = null!;

        Assert.HasCount(30, viewModel.Capabilities);
        Assert.AreEqual(string.Empty, viewModel.SearchText);
        Assert.AreEqual(1, notifications.Count(static name => name == "SelectedCapability"));
        Assert.AreEqual(2, notifications.Count(static name => name == "SearchText"));
        Assert.AreEqual(2, notifications.Count(static name => name == "Capabilities"));
    }
}