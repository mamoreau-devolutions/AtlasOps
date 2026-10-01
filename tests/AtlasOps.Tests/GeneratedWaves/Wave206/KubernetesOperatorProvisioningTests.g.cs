namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Kubernetes.KubernetesOperatorProvisioning;

[TestClass]
public sealed class KubernetesOperatorProvisioningTests
{
    private static UpdateKubernetesOperatorProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("kubernetes.kubernetesoperatorprovisioning-1", "Kubernetes Operator Provisioning", "Operations", targetState, 6, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "kubernetes.kubernetesoperatorprovisioning");

        Assert.AreEqual(206, descriptor.Wave);
        Assert.AreEqual("Kubernetes", descriptor.Area);
        Assert.AreEqual(typeof(KubernetesOperatorProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        KubernetesOperatorProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        KubernetesOperatorProvisioningValidator validator = new();
        UpdateKubernetesOperatorProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        KubernetesOperatorProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KubernetesOperatorProvisioningItem> repository = new();
        KubernetesOperatorProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KubernetesOperatorProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        KubernetesOperatorProvisioningItem? stored = await repository.GetAsync("kubernetes.kubernetesoperatorprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KubernetesOperatorProvisioningItem> repository = new();
        KubernetesOperatorProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KubernetesOperatorProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        KubernetesOperatorProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}