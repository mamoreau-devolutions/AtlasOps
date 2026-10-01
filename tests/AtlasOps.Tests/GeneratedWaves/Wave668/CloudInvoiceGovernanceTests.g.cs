namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.FinOps.CloudInvoiceGovernance;

[TestClass]
public sealed class CloudInvoiceGovernanceTests
{
    private static UpdateCloudInvoiceGovernanceCommand CreateCommand(string targetState = "Ready")
    {
        return new("finops.cloudinvoicegovernance-1", "Cloud Invoice Governance", "Operations", targetState, 8, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "finops.cloudinvoicegovernance");

        Assert.AreEqual(668, descriptor.Wave);
        Assert.AreEqual("FinOps", descriptor.Area);
        Assert.AreEqual(typeof(CloudInvoiceGovernanceItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        CloudInvoiceGovernanceValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        CloudInvoiceGovernanceValidator validator = new();
        UpdateCloudInvoiceGovernanceCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        CloudInvoiceGovernancePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CloudInvoiceGovernanceItem> repository = new();
        CloudInvoiceGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CloudInvoiceGovernanceChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        CloudInvoiceGovernanceItem? stored = await repository.GetAsync("finops.cloudinvoicegovernance-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CloudInvoiceGovernanceItem> repository = new();
        CloudInvoiceGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CloudInvoiceGovernanceChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        CloudInvoiceGovernanceViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}