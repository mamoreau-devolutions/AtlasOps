namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Messaging.MessageSchemaMonitoring;

[TestClass]
public sealed class MessageSchemaMonitoringTests
{
    private static UpdateMessageSchemaMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("messaging.messageschemamonitoring-1", "Message Schema Monitoring", "Operations", targetState, 2, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "messaging.messageschemamonitoring");

        Assert.AreEqual(432, descriptor.Wave);
        Assert.AreEqual("Messaging", descriptor.Area);
        Assert.AreEqual(typeof(MessageSchemaMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        MessageSchemaMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        MessageSchemaMonitoringValidator validator = new();
        UpdateMessageSchemaMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        MessageSchemaMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MessageSchemaMonitoringItem> repository = new();
        MessageSchemaMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MessageSchemaMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        MessageSchemaMonitoringItem? stored = await repository.GetAsync("messaging.messageschemamonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MessageSchemaMonitoringItem> repository = new();
        MessageSchemaMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MessageSchemaMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        MessageSchemaMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}