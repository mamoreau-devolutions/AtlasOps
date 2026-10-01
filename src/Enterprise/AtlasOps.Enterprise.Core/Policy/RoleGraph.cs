namespace AtlasOps.Enterprise.Core.Policy;

using AtlasOps.Enterprise.Contracts.Policy;

public sealed class RoleGraph
{
    public RoleGraphAnalysis Analyze(IReadOnlyList<RoleDefinition> definitions)
    {
        Dictionary<string, RoleDefinition> roles = new(StringComparer.OrdinalIgnoreCase);
        List<string> diagnostics = [];

        foreach (RoleDefinition role in definitions)
        {
            if (!roles.TryAdd(role.Id, role))
            {
                diagnostics.Add($"Role '{role.Id}' is defined more than once.");
            }
        }

        Dictionary<string, IReadOnlySet<string>> effective = new(StringComparer.OrdinalIgnoreCase);
        foreach (string roleId in roles.Keys.Order(StringComparer.OrdinalIgnoreCase))
        {
            HashSet<string> resolved = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> visiting = new(StringComparer.OrdinalIgnoreCase);
            this.Resolve(roleId, roles, resolved, visiting, diagnostics);
            effective[roleId] = resolved;
        }

        return new(effective, diagnostics.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
    }

    public IReadOnlySet<string> Expand(IReadOnlySet<string> assignedRoles, RoleGraphAnalysis analysis)
    {
        HashSet<string> expanded = new(assignedRoles, StringComparer.OrdinalIgnoreCase);
        foreach (string role in assignedRoles)
        {
            if (analysis.EffectiveRoles.TryGetValue(role, out IReadOnlySet<string>? inherited))
            {
                expanded.UnionWith(inherited);
            }
        }

        return expanded;
    }

    private void Resolve(
        string roleId,
        IReadOnlyDictionary<string, RoleDefinition> roles,
        ISet<string> resolved,
        ISet<string> visiting,
        ICollection<string> diagnostics)
    {
        if (!roles.TryGetValue(roleId, out RoleDefinition? role))
        {
            diagnostics.Add($"Inherited role '{roleId}' does not exist.");
            return;
        }

        if (!visiting.Add(roleId))
        {
            diagnostics.Add($"Role inheritance contains a cycle at '{roleId}'.");
            return;
        }

        resolved.Add(roleId);
        foreach (string inheritedRoleId in role.InheritedRoleIds.Order(StringComparer.OrdinalIgnoreCase))
        {
            if (visiting.Contains(inheritedRoleId))
            {
                diagnostics.Add($"Role inheritance contains a cycle at '{inheritedRoleId}'.");
                continue;
            }

            if (resolved.Contains(inheritedRoleId))
            {
                continue;
            }

            this.Resolve(inheritedRoleId, roles, resolved, visiting, diagnostics);
        }

        visiting.Remove(roleId);
    }
}
