namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Network.NetworkDnsZoneRecovery;

[TestClass]
public sealed class NetworkDnsZoneRecoveryTests
{
    private static UpdateNetworkDnsZoneRecoveryCommand CreateCommand(string targetState = "Ready")
    {
        return new("network.networkdnszonerecovery-1", "Network Dns Zone Recovery", "Operations", targetState, 5, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "network.networkdnszonerecovery");

        Assert.AreEqual(235, descriptor.Wave);
        Assert.AreEqual("Network", descriptor.Area);
        Assert.AreEqual(typeof(NetworkDnsZoneRecoveryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        NetworkDnsZoneRecoveryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        NetworkDnsZoneRecoveryValidator validator = new();
        UpdateNetworkDnsZoneRecoveryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        NetworkDnsZoneRecoveryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<NetworkDnsZoneRecoveryItem> repository = new();
        NetworkDnsZoneRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<NetworkDnsZoneRecoveryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        NetworkDnsZoneRecoveryItem? stored = await repository.GetAsync("network.networkdnszonerecovery-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<NetworkDnsZoneRecoveryItem> repository = new();
        NetworkDnsZoneRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<NetworkDnsZoneRecoveryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        NetworkDnsZoneRecoveryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}