namespace AtlasOps.Enterprise.Core.Policy;

using AtlasOps.Enterprise.Contracts.Policy;

public sealed class PolicyAnalyzer
{
    public IReadOnlyList<string> Analyze(PolicySet policySet)
    {
        List<string> diagnostics = [];
        HashSet<string> identifiers = new(StringComparer.OrdinalIgnoreCase);

        foreach (PolicyRule rule in policySet.Rules)
        {
            if (!identifiers.Add(rule.Id))
            {
                diagnostics.Add($"Rule identifier '{rule.Id}' is duplicated.");
            }

            if (rule.Actions.Count == 0)
            {
                diagnostics.Add($"Rule '{rule.Id}' has no action scope.");
            }

            if (rule.ResourceTypes.Count == 0)
            {
                diagnostics.Add($"Rule '{rule.Id}' has no resource scope.");
            }

            if (rule.Priority < 0)
            {
                diagnostics.Add($"Rule '{rule.Id}' has a negative priority.");
            }
        }

        foreach (PolicyRule allow in policySet.Rules.Where(static rule => rule.Effect == PolicyEffect.Allow))
        {
            PolicyRule? overlappingDeny = policySet.Rules
                .Where(rule => rule.Effect == PolicyEffect.Deny && rule.Priority >= allow.Priority)
                .OrderByDescending(static rule => rule.Priority)
                .FirstOrDefault(deny => ScopesOverlap(allow, deny));
            if (overlappingDeny is not null)
            {
                diagnostics.Add($"Allow rule '{allow.Id}' overlaps deny rule '{overlappingDeny.Id}' at priority {overlappingDeny.Priority}; deny precedence will apply.");
            }
        }

        return diagnostics;
    }

    private static bool ScopesOverlap(PolicyRule left, PolicyRule right)
    {
        bool actionOverlap = left.Actions.Contains("*") || right.Actions.Contains("*") || left.Actions.Overlaps(right.Actions);
        bool resourceOverlap = left.ResourceTypes.Contains("*") || right.ResourceTypes.Contains("*") || left.ResourceTypes.Overlaps(right.ResourceTypes);
        bool roleOverlap = left.Roles.Count == 0 || right.Roles.Count == 0 || left.Roles.Overlaps(right.Roles);
        return actionOverlap && resourceOverlap && roleOverlap;
    }
}
