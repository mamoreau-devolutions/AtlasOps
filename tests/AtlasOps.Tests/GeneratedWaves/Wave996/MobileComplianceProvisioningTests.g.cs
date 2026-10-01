namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Mobile.MobileComplianceProvisioning;

[TestClass]
public sealed class MobileComplianceProvisioningTests
{
    private static UpdateMobileComplianceProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("mobile.mobilecomplianceprovisioning-1", "Mobile Compliance Provisioning", "Operations", targetState, 6, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "mobile.mobilecomplianceprovisioning");

        Assert.AreEqual(996, descriptor.Wave);
        Assert.AreEqual("Mobile", descriptor.Area);
        Assert.AreEqual(typeof(MobileComplianceProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        MobileComplianceProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        MobileComplianceProvisioningValidator validator = new();
        UpdateMobileComplianceProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        MobileComplianceProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MobileComplianceProvisioningItem> repository = new();
        MobileComplianceProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MobileComplianceProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        MobileComplianceProvisioningItem? stored = await repository.GetAsync("mobile.mobilecomplianceprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MobileComplianceProvisioningItem> repository = new();
        MobileComplianceProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MobileComplianceProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        MobileComplianceProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}