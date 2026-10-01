namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.ServiceManagement.KnowledgeArticleOptimization;

[TestClass]
public sealed class KnowledgeArticleOptimizationTests
{
    private static UpdateKnowledgeArticleOptimizationCommand CreateCommand(string targetState = "Ready")
    {
        return new("servicemanagement.knowledgearticleoptimization-1", "Knowledge Article Optimization", "Operations", targetState, 4, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "servicemanagement.knowledgearticleoptimization");

        Assert.AreEqual(744, descriptor.Wave);
        Assert.AreEqual("ServiceManagement", descriptor.Area);
        Assert.AreEqual(typeof(KnowledgeArticleOptimizationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        KnowledgeArticleOptimizationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        KnowledgeArticleOptimizationValidator validator = new();
        UpdateKnowledgeArticleOptimizationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        KnowledgeArticleOptimizationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KnowledgeArticleOptimizationItem> repository = new();
        KnowledgeArticleOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KnowledgeArticleOptimizationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        KnowledgeArticleOptimizationItem? stored = await repository.GetAsync("servicemanagement.knowledgearticleoptimization-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KnowledgeArticleOptimizationItem> repository = new();
        KnowledgeArticleOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KnowledgeArticleOptimizationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        KnowledgeArticleOptimizationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}