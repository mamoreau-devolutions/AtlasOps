namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Platform.LocalizationCatalog;

[TestClass]
public sealed class LocalizationCatalogTests
{
    private static UpdateLocalizationCatalogCommand CreateCommand(string targetState = "Ready")
    {
        return new("platform.localizationcatalog-1", "Localization Catalog", "Operations", targetState, 8, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "platform.localizationcatalog");

        Assert.AreEqual(18, descriptor.Wave);
        Assert.AreEqual("Platform", descriptor.Area);
        Assert.AreEqual(typeof(LocalizationCatalogItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        LocalizationCatalogValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        LocalizationCatalogValidator validator = new();
        UpdateLocalizationCatalogCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        LocalizationCatalogPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<LocalizationCatalogItem> repository = new();
        LocalizationCatalogService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<LocalizationCatalogChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        LocalizationCatalogItem? stored = await repository.GetAsync("platform.localizationcatalog-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<LocalizationCatalogItem> repository = new();
        LocalizationCatalogService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<LocalizationCatalogChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        LocalizationCatalogViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}