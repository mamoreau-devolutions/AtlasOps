namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Security.SecurityIdentityMonitoring;

[TestClass]
public sealed class SecurityIdentityMonitoringTests
{
    private static UpdateSecurityIdentityMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("security.securityidentitymonitoring-1", "Security Identity Monitoring", "Operations", targetState, 2, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "security.securityidentitymonitoring");

        Assert.AreEqual(292, descriptor.Wave);
        Assert.AreEqual("Security", descriptor.Area);
        Assert.AreEqual(typeof(SecurityIdentityMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        SecurityIdentityMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        SecurityIdentityMonitoringValidator validator = new();
        UpdateSecurityIdentityMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        SecurityIdentityMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<SecurityIdentityMonitoringItem> repository = new();
        SecurityIdentityMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<SecurityIdentityMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        SecurityIdentityMonitoringItem? stored = await repository.GetAsync("security.securityidentitymonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<SecurityIdentityMonitoringItem> repository = new();
        SecurityIdentityMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<SecurityIdentityMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        SecurityIdentityMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}