namespace AtlasOps.Integrations.Kubernetes.Core;

using AtlasOps.Integrations.Kubernetes.Contracts;

public sealed class KubernetesReconciliationService
{
    public KubernetesReconciliationPlan CreatePlan(
        IReadOnlyList<KubernetesResourceSnapshot> desired,
        IReadOnlyList<KubernetesResourceSnapshot> observed)
    {
        Dictionary<KubernetesResourceKey, KubernetesResourceSnapshot> desiredByKey =
            desired.ToDictionary(static item => item.Key);
        Dictionary<KubernetesResourceKey, KubernetesResourceSnapshot> observedByKey =
            observed.ToDictionary(static item => item.Key);
        List<string> diagnostics = [];

        IReadOnlyList<KubernetesResourceKey> order = TopologicalOrder(desired, diagnostics);
        Dictionary<KubernetesResourceKey, int> orderByKey = order
            .Select(static (key, index) => new KeyValuePair<KubernetesResourceKey, int>(key, index))
            .ToDictionary();
        List<KubernetesReconciliationAction> actions = [];

        foreach (KubernetesResourceKey key in order)
        {
            KubernetesResourceSnapshot target = desiredByKey[key];
            if (!observedByKey.TryGetValue(key, out KubernetesResourceSnapshot? current))
            {
                actions.Add(new KubernetesReconciliationAction(
                    key,
                    KubernetesReconciliationActionKind.Create,
                    orderByKey[key],
                    target.DesiredValues.Keys.Order(StringComparer.Ordinal).ToArray(),
                    "Resource does not exist."));
                continue;
            }

            string[] changed = target.DesiredValues
                .Where(pair =>
                    !current.ObservedValues.TryGetValue(pair.Key, out string? value) ||
                    !string.Equals(value, pair.Value, StringComparison.Ordinal))
                .Select(static pair => pair.Key)
                .Order(StringComparer.Ordinal)
                .ToArray();
            actions.Add(new KubernetesReconciliationAction(
                key,
                changed.Length == 0
                    ? KubernetesReconciliationActionKind.NoChange
                    : KubernetesReconciliationActionKind.Update,
                orderByKey[key],
                changed,
                changed.Length == 0 ? "Desired state is satisfied." : "Observed values differ."));
        }

        int deleteOrder = actions.Count;
        actions.AddRange(
            observed
                .Where(item => !desiredByKey.ContainsKey(item.Key))
                .OrderByDescending(item => DependencyDepth(item, observedByKey, []))
                .Select(item => new KubernetesReconciliationAction(
                    item.Key,
                    KubernetesReconciliationActionKind.Delete,
                    deleteOrder++,
                    [],
                    "Resource is absent from desired state.")));

        return new KubernetesReconciliationPlan(actions, diagnostics);
    }

    private static IReadOnlyList<KubernetesResourceKey> TopologicalOrder(
        IReadOnlyList<KubernetesResourceSnapshot> resources,
        List<string> diagnostics)
    {
        Dictionary<KubernetesResourceKey, KubernetesResourceSnapshot> byKey =
            resources.ToDictionary(static item => item.Key);
        List<KubernetesResourceKey> ordered = [];
        HashSet<KubernetesResourceKey> visiting = [];
        HashSet<KubernetesResourceKey> visited = [];

        foreach (KubernetesResourceSnapshot resource in resources.OrderBy(static item => item.Key.Name))
        {
            Visit(resource, byKey, visiting, visited, ordered, diagnostics);
        }

        return ordered;
    }

    private static void Visit(
        KubernetesResourceSnapshot resource,
        IReadOnlyDictionary<KubernetesResourceKey, KubernetesResourceSnapshot> byKey,
        HashSet<KubernetesResourceKey> visiting,
        HashSet<KubernetesResourceKey> visited,
        List<KubernetesResourceKey> ordered,
        List<string> diagnostics)
    {
        if (visited.Contains(resource.Key))
        {
            return;
        }

        if (!visiting.Add(resource.Key))
        {
            diagnostics.Add($"Dependency cycle detected at '{resource.Key.Name}'.");
            return;
        }

        foreach (KubernetesResourceKey dependency in resource.Dependencies)
        {
            if (byKey.TryGetValue(dependency, out KubernetesResourceSnapshot? dependencyResource))
            {
                Visit(dependencyResource, byKey, visiting, visited, ordered, diagnostics);
            }
            else
            {
                diagnostics.Add(
                    $"Resource '{resource.Key.Name}' depends on missing resource '{dependency.Name}'.");
            }
        }

        visiting.Remove(resource.Key);
        if (visited.Add(resource.Key))
        {
            ordered.Add(resource.Key);
        }
    }

    private static int DependencyDepth(
        KubernetesResourceSnapshot resource,
        IReadOnlyDictionary<KubernetesResourceKey, KubernetesResourceSnapshot> resources,
        HashSet<KubernetesResourceKey> visited)
    {
        if (!visited.Add(resource.Key))
        {
            return 0;
        }

        int depth = resource.Dependencies
            .Where(resources.ContainsKey)
            .Select(key => DependencyDepth(resources[key], resources, visited))
            .DefaultIfEmpty(0)
            .Max();
        return depth + 1;
    }
}
