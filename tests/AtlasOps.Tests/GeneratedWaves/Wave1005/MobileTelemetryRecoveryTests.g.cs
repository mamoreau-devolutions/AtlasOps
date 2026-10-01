namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Mobile.MobileTelemetryRecovery;

[TestClass]
public sealed class MobileTelemetryRecoveryTests
{
    private static UpdateMobileTelemetryRecoveryCommand CreateCommand(string targetState = "Ready")
    {
        return new("mobile.mobiletelemetryrecovery-1", "Mobile Telemetry Recovery", "Operations", targetState, 5, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "mobile.mobiletelemetryrecovery");

        Assert.AreEqual(1005, descriptor.Wave);
        Assert.AreEqual("Mobile", descriptor.Area);
        Assert.AreEqual(typeof(MobileTelemetryRecoveryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        MobileTelemetryRecoveryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        MobileTelemetryRecoveryValidator validator = new();
        UpdateMobileTelemetryRecoveryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        MobileTelemetryRecoveryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MobileTelemetryRecoveryItem> repository = new();
        MobileTelemetryRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MobileTelemetryRecoveryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        MobileTelemetryRecoveryItem? stored = await repository.GetAsync("mobile.mobiletelemetryrecovery-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MobileTelemetryRecoveryItem> repository = new();
        MobileTelemetryRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MobileTelemetryRecoveryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        MobileTelemetryRecoveryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}