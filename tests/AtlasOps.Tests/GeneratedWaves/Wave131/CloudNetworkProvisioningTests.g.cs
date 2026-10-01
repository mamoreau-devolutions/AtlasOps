namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Cloud.CloudNetworkProvisioning;

[TestClass]
public sealed class CloudNetworkProvisioningTests
{
    private static UpdateCloudNetworkProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("cloud.cloudnetworkprovisioning-1", "Cloud Network Provisioning", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "cloud.cloudnetworkprovisioning");

        Assert.AreEqual(131, descriptor.Wave);
        Assert.AreEqual("Cloud", descriptor.Area);
        Assert.AreEqual(typeof(CloudNetworkProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        CloudNetworkProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        CloudNetworkProvisioningValidator validator = new();
        UpdateCloudNetworkProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        CloudNetworkProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CloudNetworkProvisioningItem> repository = new();
        CloudNetworkProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CloudNetworkProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        CloudNetworkProvisioningItem? stored = await repository.GetAsync("cloud.cloudnetworkprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CloudNetworkProvisioningItem> repository = new();
        CloudNetworkProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CloudNetworkProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        CloudNetworkProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}