namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Incidents.PostmortemReview;

[TestClass]
public sealed class PostmortemReviewTests
{
    private static UpdatePostmortemReviewCommand CreateCommand(string targetState = "Ready")
    {
        return new("incidents.postmortemreview-1", "Postmortem Review", "Operations", targetState, 5, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "incidents.postmortemreview");

        Assert.AreEqual(5, descriptor.Wave);
        Assert.AreEqual("Incidents", descriptor.Area);
        Assert.AreEqual(typeof(PostmortemReviewItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        PostmortemReviewValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        PostmortemReviewValidator validator = new();
        UpdatePostmortemReviewCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        PostmortemReviewPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<PostmortemReviewItem> repository = new();
        PostmortemReviewService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<PostmortemReviewChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        PostmortemReviewItem? stored = await repository.GetAsync("incidents.postmortemreview-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<PostmortemReviewItem> repository = new();
        PostmortemReviewService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<PostmortemReviewChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        PostmortemReviewViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}