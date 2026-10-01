namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Api.ApiVersionProvisioning;

[TestClass]
public sealed class ApiVersionProvisioningTests
{
    private static UpdateApiVersionProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("api.apiversionprovisioning-1", "Api Version Provisioning", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "api.apiversionprovisioning");

        Assert.AreEqual(491, descriptor.Wave);
        Assert.AreEqual("Api", descriptor.Area);
        Assert.AreEqual(typeof(ApiVersionProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ApiVersionProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ApiVersionProvisioningValidator validator = new();
        UpdateApiVersionProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ApiVersionProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ApiVersionProvisioningItem> repository = new();
        ApiVersionProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ApiVersionProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ApiVersionProvisioningItem? stored = await repository.GetAsync("api.apiversionprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ApiVersionProvisioningItem> repository = new();
        ApiVersionProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ApiVersionProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ApiVersionProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}