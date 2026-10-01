namespace AtlasOps.Enterprise.Core.Incidents;

using AtlasOps.Enterprise.Contracts.Incidents;

public sealed class ServiceImpactAnalyzer
{
    public IReadOnlyList<ServiceImpact> FindImpactedServices(
        string failedServiceId,
        IReadOnlyList<ServiceDependency> dependencies)
    {
        ILookup<string, ServiceDependency> dependents = dependencies.ToLookup(
            static dependency => dependency.DependsOnServiceId,
            StringComparer.OrdinalIgnoreCase);
        Queue<ServiceImpact> pending = new();
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase) { failedServiceId };
        List<ServiceImpact> impacts = [];

        pending.Enqueue(new(failedServiceId, 0, false, [failedServiceId]));
        while (pending.TryDequeue(out ServiceImpact? current))
        {
            foreach (ServiceDependency dependency in dependents[current.ServiceId]
                         .OrderBy(static item => item.ServiceId, StringComparer.OrdinalIgnoreCase))
            {
                if (!visited.Add(dependency.ServiceId))
                {
                    continue;
                }

                string[] path = [.. current.Path, dependency.ServiceId];
                ServiceImpact impact = new(
                    dependency.ServiceId,
                    current.Distance + 1,
                    current.IsCriticalPath || dependency.IsCritical,
                    path);
                impacts.Add(impact);
                pending.Enqueue(impact);
            }
        }

        return impacts
            .OrderBy(static impact => impact.Distance)
            .ThenBy(static impact => impact.ServiceId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
