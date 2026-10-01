namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Security.SecurityScanProvisioning;

[TestClass]
public sealed class SecurityScanProvisioningTests
{
    private static UpdateSecurityScanProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("security.securityscanprovisioning-1", "Security Scan Provisioning", "Operations", targetState, 6, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "security.securityscanprovisioning");

        Assert.AreEqual(266, descriptor.Wave);
        Assert.AreEqual("Security", descriptor.Area);
        Assert.AreEqual(typeof(SecurityScanProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        SecurityScanProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        SecurityScanProvisioningValidator validator = new();
        UpdateSecurityScanProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        SecurityScanProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<SecurityScanProvisioningItem> repository = new();
        SecurityScanProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<SecurityScanProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        SecurityScanProvisioningItem? stored = await repository.GetAsync("security.securityscanprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<SecurityScanProvisioningItem> repository = new();
        SecurityScanProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<SecurityScanProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        SecurityScanProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}