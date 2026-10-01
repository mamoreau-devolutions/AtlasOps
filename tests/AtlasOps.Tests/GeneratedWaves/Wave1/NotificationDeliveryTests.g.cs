namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Platform.NotificationDelivery;

[TestClass]
public sealed class NotificationDeliveryTests
{
    private static UpdateNotificationDeliveryCommand CreateCommand(string targetState = "Ready")
    {
        return new("platform.notificationdelivery-1", "Notification Delivery", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "platform.notificationdelivery");

        Assert.AreEqual(1, descriptor.Wave);
        Assert.AreEqual("Platform", descriptor.Area);
        Assert.AreEqual(typeof(NotificationDeliveryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        NotificationDeliveryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        NotificationDeliveryValidator validator = new();
        UpdateNotificationDeliveryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        NotificationDeliveryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<NotificationDeliveryItem> repository = new();
        NotificationDeliveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<NotificationDeliveryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        NotificationDeliveryItem? stored = await repository.GetAsync("platform.notificationdelivery-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<NotificationDeliveryItem> repository = new();
        NotificationDeliveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<NotificationDeliveryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        NotificationDeliveryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}