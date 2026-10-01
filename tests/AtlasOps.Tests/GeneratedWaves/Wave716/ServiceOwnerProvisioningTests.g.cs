namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.ServiceManagement.ServiceOwnerProvisioning;

[TestClass]
public sealed class ServiceOwnerProvisioningTests
{
    private static UpdateServiceOwnerProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("servicemanagement.serviceownerprovisioning-1", "Service Owner Provisioning", "Operations", targetState, 6, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "servicemanagement.serviceownerprovisioning");

        Assert.AreEqual(716, descriptor.Wave);
        Assert.AreEqual("ServiceManagement", descriptor.Area);
        Assert.AreEqual(typeof(ServiceOwnerProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ServiceOwnerProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ServiceOwnerProvisioningValidator validator = new();
        UpdateServiceOwnerProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ServiceOwnerProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ServiceOwnerProvisioningItem> repository = new();
        ServiceOwnerProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ServiceOwnerProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ServiceOwnerProvisioningItem? stored = await repository.GetAsync("servicemanagement.serviceownerprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ServiceOwnerProvisioningItem> repository = new();
        ServiceOwnerProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ServiceOwnerProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ServiceOwnerProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}