namespace AtlasOps.Integrations.Kubernetes.Contracts;

public sealed record KubernetesResourceKey(
    string Cluster,
    string Namespace,
    string Kind,
    string Name);

public sealed record KubernetesResourceSnapshot(
    KubernetesResourceKey Key,
    string ApiVersion,
    string Revision,
    IReadOnlyDictionary<string, string> DesiredValues,
    IReadOnlyDictionary<string, string> ObservedValues,
    IReadOnlyList<KubernetesResourceKey> Dependencies);

public enum KubernetesReconciliationActionKind
{
    Create,
    Update,
    Delete,
    Wait,
    NoChange,
}

public sealed record KubernetesReconciliationAction(
    KubernetesResourceKey Key,
    KubernetesReconciliationActionKind Kind,
    int Order,
    IReadOnlyList<string> ChangedFields,
    string Reason);

public sealed record KubernetesReconciliationPlan(
    IReadOnlyList<KubernetesReconciliationAction> Actions,
    IReadOnlyList<string> Diagnostics);
