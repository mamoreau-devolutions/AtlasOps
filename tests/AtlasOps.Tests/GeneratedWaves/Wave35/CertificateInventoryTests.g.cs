namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Inventory.CertificateInventory;

[TestClass]
public sealed class CertificateInventoryTests
{
    private static UpdateCertificateInventoryCommand CreateCommand(string targetState = "Ready")
    {
        return new("inventory.certificateinventory-1", "Certificate Inventory", "Operations", targetState, 5, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "inventory.certificateinventory");

        Assert.AreEqual(35, descriptor.Wave);
        Assert.AreEqual("Inventory", descriptor.Area);
        Assert.AreEqual(typeof(CertificateInventoryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        CertificateInventoryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        CertificateInventoryValidator validator = new();
        UpdateCertificateInventoryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        CertificateInventoryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CertificateInventoryItem> repository = new();
        CertificateInventoryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CertificateInventoryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        CertificateInventoryItem? stored = await repository.GetAsync("inventory.certificateinventory-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CertificateInventoryItem> repository = new();
        CertificateInventoryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CertificateInventoryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        CertificateInventoryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}