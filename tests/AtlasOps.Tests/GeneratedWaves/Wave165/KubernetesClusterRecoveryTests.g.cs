namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Kubernetes.KubernetesClusterRecovery;

[TestClass]
public sealed class KubernetesClusterRecoveryTests
{
    private static UpdateKubernetesClusterRecoveryCommand CreateCommand(string targetState = "Ready")
    {
        return new("kubernetes.kubernetesclusterrecovery-1", "Kubernetes Cluster Recovery", "Operations", targetState, 5, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "kubernetes.kubernetesclusterrecovery");

        Assert.AreEqual(165, descriptor.Wave);
        Assert.AreEqual("Kubernetes", descriptor.Area);
        Assert.AreEqual(typeof(KubernetesClusterRecoveryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        KubernetesClusterRecoveryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        KubernetesClusterRecoveryValidator validator = new();
        UpdateKubernetesClusterRecoveryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        KubernetesClusterRecoveryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KubernetesClusterRecoveryItem> repository = new();
        KubernetesClusterRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KubernetesClusterRecoveryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        KubernetesClusterRecoveryItem? stored = await repository.GetAsync("kubernetes.kubernetesclusterrecovery-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KubernetesClusterRecoveryItem> repository = new();
        KubernetesClusterRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KubernetesClusterRecoveryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        KubernetesClusterRecoveryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}