namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.Kubernetes.Contracts;
using AtlasOps.Integrations.Kubernetes.Core;

[TestClass]
public sealed class KubernetesReconciliationServiceTests
{
    [TestMethod]
    public void CreatePlan_CreatesDependenciesBeforeDependentsAndSortsChangedFields()
    {
        KubernetesReconciliationService service = new();
        KubernetesResourceKey dependencyKey = Key("config");
        KubernetesResourceKey workloadKey = Key("workload");
        KubernetesResourceSnapshot dependency = Snapshot(dependencyKey, new Dictionary<string, string> { ["b"] = "2", ["a"] = "1" });
        KubernetesResourceSnapshot workload = Snapshot(workloadKey, new Dictionary<string, string> { ["image"] = "v2" }, [dependencyKey]);
        KubernetesResourceSnapshot observedWorkload = Snapshot(
            workloadKey,
            new Dictionary<string, string>(),
            observed: new Dictionary<string, string> { ["image"] = "v1" });

        KubernetesReconciliationPlan result = service.CreatePlan(
            [workload, dependency],
            [observedWorkload]);

        CollectionAssert.AreEqual(
            new[] { dependencyKey, workloadKey },
            result.Actions.Select(static action => action.Key).ToArray());
        Assert.AreEqual(KubernetesReconciliationActionKind.Create, result.Actions[0].Kind);
        CollectionAssert.AreEqual(new[] { "a", "b" }, result.Actions[0].ChangedFields.ToArray());
        Assert.AreEqual(KubernetesReconciliationActionKind.Update, result.Actions[1].Kind);
        CollectionAssert.AreEqual(new[] { "image" }, result.Actions[1].ChangedFields.ToArray());
    }

    [TestMethod]
    public void CreatePlan_SatisfiedResourceProducesNoChange()
    {
        KubernetesReconciliationService service = new();
        KubernetesResourceKey key = Key("config");
        KubernetesResourceSnapshot desired = Snapshot(key, new Dictionary<string, string> { ["value"] = "same" });
        KubernetesResourceSnapshot observed = Snapshot(
            key,
            new Dictionary<string, string>(),
            observed: new Dictionary<string, string> { ["value"] = "same" });

        KubernetesReconciliationPlan result = service.CreatePlan([desired], [observed]);

        Assert.AreEqual(KubernetesReconciliationActionKind.NoChange, result.Actions.Single().Kind);
        Assert.IsEmpty(result.Actions.Single().ChangedFields);
        Assert.AreEqual("Desired state is satisfied.", result.Actions.Single().Reason);
    }

    [TestMethod]
    public void CreatePlan_MissingDependencyAndCycle_ReturnConcreteDiagnostics()
    {
        KubernetesReconciliationService service = new();
        KubernetesResourceKey a = Key("a");
        KubernetesResourceKey b = Key("b");
        KubernetesResourceKey missing = Key("missing");

        KubernetesReconciliationPlan result = service.CreatePlan(
            [
                Snapshot(a, new Dictionary<string, string>(), [b, missing]),
                Snapshot(b, new Dictionary<string, string>(), [a]),
            ],
            []);

        Assert.Contains("Dependency cycle detected at 'a'.", result.Diagnostics);
        Assert.Contains("Resource 'a' depends on missing resource 'missing'.", result.Diagnostics);
    }

    [TestMethod]
    public void CreatePlan_DeletesDependentBeforeDependency()
    {
        KubernetesReconciliationService service = new();
        KubernetesResourceKey dependency = Key("dependency");
        KubernetesResourceKey dependent = Key("dependent");

        KubernetesReconciliationPlan result = service.CreatePlan(
            [],
            [
                Snapshot(dependency, new Dictionary<string, string>()),
                Snapshot(dependent, new Dictionary<string, string>(), [dependency]),
            ]);

        CollectionAssert.AreEqual(
            new[] { dependent, dependency },
            result.Actions.Select(static action => action.Key).ToArray());
        Assert.IsTrue(result.Actions.All(static action => action.Kind == KubernetesReconciliationActionKind.Delete));
    }

    private static KubernetesResourceKey Key(string name)
    {
        return new KubernetesResourceKey("cluster", "namespace", "Kind", name);
    }

    private static KubernetesResourceSnapshot Snapshot(
        KubernetesResourceKey key,
        IReadOnlyDictionary<string, string> desired,
        IReadOnlyList<KubernetesResourceKey>? dependencies = null,
        IReadOnlyDictionary<string, string>? observed = null)
    {
        return new KubernetesResourceSnapshot(
            key,
            "v1",
            "1",
            desired,
            observed ?? new Dictionary<string, string>(),
            dependencies ?? []);
    }
}
