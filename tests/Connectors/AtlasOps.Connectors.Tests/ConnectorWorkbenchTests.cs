namespace AtlasOps.Connectors.Tests;

using AtlasOps.Connectors.Avalonia;
using AtlasOps.Connectors.Contracts;

[TestClass]
public sealed class ConnectorWorkbenchTests
{
    [TestMethod]
    public void SearchText_KindFilter_NarrowsConnectorList()
    {
        ConnectorWorkbenchViewModel viewModel = new();

        viewModel.SearchText = "Observability";

        Assert.HasCount(1, viewModel.Connectors);
        Assert.AreEqual(ConnectorKind.Observability, viewModel.Connectors[0].Kind);
    }

    [TestMethod]
    public async Task ExecuteSelectedAsync_SelectedConnector_AddsAuditHistory()
    {
        ConnectorWorkbenchViewModel viewModel = new();
        Assert.IsNotNull(viewModel.SelectedConnector);

        await viewModel.ExecuteSelectedAsync();

        Assert.HasCount(1, viewModel.Executions);
        Assert.AreEqual(ConnectorExecutionStatus.Succeeded, viewModel.Executions[0].Status);
    }

    [TestMethod]
    public async Task RefreshAsync_ReferenceConnectors_AddsHealthRows()
    {
        ConnectorWorkbenchViewModel viewModel = new();

        await viewModel.RefreshAsync();

        Assert.HasCount(5, viewModel.Health);
        StringAssert.Contains(viewModel.StatusMessage, "5");
    }
}
