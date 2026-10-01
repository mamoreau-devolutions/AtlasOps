namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Messaging.MessageSubscriptionGovernance;

[TestClass]
public sealed class MessageSubscriptionGovernanceTests
{
    private static UpdateMessageSubscriptionGovernanceCommand CreateCommand(string targetState = "Ready")
    {
        return new("messaging.messagesubscriptiongovernance-1", "Message Subscription Governance", "Operations", targetState, 8, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "messaging.messagesubscriptiongovernance");

        Assert.AreEqual(428, descriptor.Wave);
        Assert.AreEqual("Messaging", descriptor.Area);
        Assert.AreEqual(typeof(MessageSubscriptionGovernanceItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        MessageSubscriptionGovernanceValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        MessageSubscriptionGovernanceValidator validator = new();
        UpdateMessageSubscriptionGovernanceCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        MessageSubscriptionGovernancePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MessageSubscriptionGovernanceItem> repository = new();
        MessageSubscriptionGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MessageSubscriptionGovernanceChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        MessageSubscriptionGovernanceItem? stored = await repository.GetAsync("messaging.messagesubscriptiongovernance-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MessageSubscriptionGovernanceItem> repository = new();
        MessageSubscriptionGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MessageSubscriptionGovernanceChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        MessageSubscriptionGovernanceViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}