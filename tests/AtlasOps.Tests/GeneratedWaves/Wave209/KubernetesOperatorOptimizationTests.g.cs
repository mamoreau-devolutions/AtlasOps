namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Kubernetes.KubernetesOperatorOptimization;

[TestClass]
public sealed class KubernetesOperatorOptimizationTests
{
    private static UpdateKubernetesOperatorOptimizationCommand CreateCommand(string targetState = "Ready")
    {
        return new("kubernetes.kubernetesoperatoroptimization-1", "Kubernetes Operator Optimization", "Operations", targetState, 9, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "kubernetes.kubernetesoperatoroptimization");

        Assert.AreEqual(209, descriptor.Wave);
        Assert.AreEqual("Kubernetes", descriptor.Area);
        Assert.AreEqual(typeof(KubernetesOperatorOptimizationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        KubernetesOperatorOptimizationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        KubernetesOperatorOptimizationValidator validator = new();
        UpdateKubernetesOperatorOptimizationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        KubernetesOperatorOptimizationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KubernetesOperatorOptimizationItem> repository = new();
        KubernetesOperatorOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KubernetesOperatorOptimizationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        KubernetesOperatorOptimizationItem? stored = await repository.GetAsync("kubernetes.kubernetesoperatoroptimization-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KubernetesOperatorOptimizationItem> repository = new();
        KubernetesOperatorOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KubernetesOperatorOptimizationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        KubernetesOperatorOptimizationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}