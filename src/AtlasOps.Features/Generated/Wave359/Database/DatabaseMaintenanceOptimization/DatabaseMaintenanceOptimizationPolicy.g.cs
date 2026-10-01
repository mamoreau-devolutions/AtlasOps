namespace AtlasOps.Features.Database.DatabaseMaintenanceOptimization;

public sealed class DatabaseMaintenanceOptimizationPolicy
{
    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> Transitions =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Draft"] = new HashSet<string>(["Ready", "Cancelled"], StringComparer.OrdinalIgnoreCase),
            ["Ready"] = new HashSet<string>(["Running", "Cancelled"], StringComparer.OrdinalIgnoreCase),
            ["Running"] = new HashSet<string>(["Completed", "Failed", "Paused"], StringComparer.OrdinalIgnoreCase),
            ["Paused"] = new HashSet<string>(["Running", "Cancelled"], StringComparer.OrdinalIgnoreCase),
            ["Failed"] = new HashSet<string>(["Ready", "Cancelled"], StringComparer.OrdinalIgnoreCase),
            ["Completed"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ["Cancelled"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
        };

    public bool CanTransition(string currentState, string targetState)
    {
        return Transitions.TryGetValue(currentState, out IReadOnlySet<string>? targets) && targets.Contains(targetState);
    }

    public IReadOnlyList<string> GetAvailableTransitions(string currentState)
    {
        return Transitions.TryGetValue(currentState, out IReadOnlySet<string>? targets)
            ? targets.Order(StringComparer.OrdinalIgnoreCase).ToArray()
            : [];
    }
}