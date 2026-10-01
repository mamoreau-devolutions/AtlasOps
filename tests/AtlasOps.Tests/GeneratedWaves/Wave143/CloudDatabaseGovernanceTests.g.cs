namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Cloud.CloudDatabaseGovernance;

[TestClass]
public sealed class CloudDatabaseGovernanceTests
{
    private static UpdateCloudDatabaseGovernanceCommand CreateCommand(string targetState = "Ready")
    {
        return new("cloud.clouddatabasegovernance-1", "Cloud Database Governance", "Operations", targetState, 3, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "cloud.clouddatabasegovernance");

        Assert.AreEqual(143, descriptor.Wave);
        Assert.AreEqual("Cloud", descriptor.Area);
        Assert.AreEqual(typeof(CloudDatabaseGovernanceItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        CloudDatabaseGovernanceValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        CloudDatabaseGovernanceValidator validator = new();
        UpdateCloudDatabaseGovernanceCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        CloudDatabaseGovernancePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CloudDatabaseGovernanceItem> repository = new();
        CloudDatabaseGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CloudDatabaseGovernanceChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        CloudDatabaseGovernanceItem? stored = await repository.GetAsync("cloud.clouddatabasegovernance-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CloudDatabaseGovernanceItem> repository = new();
        CloudDatabaseGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CloudDatabaseGovernanceChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        CloudDatabaseGovernanceViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}