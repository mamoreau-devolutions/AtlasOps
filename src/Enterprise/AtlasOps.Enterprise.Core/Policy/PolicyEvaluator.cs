namespace AtlasOps.Enterprise.Core.Policy;

using AtlasOps.Enterprise.Contracts.Policy;

public sealed class PolicyEvaluator
{
    private readonly RoleGraph roleGraph = new();
    private readonly PolicyConditionEvaluator conditionEvaluator = new();

    public PolicyDecision Evaluate(
        PolicyRequest request,
        PolicySet policySet,
        IReadOnlyList<RoleDefinition> roleDefinitions)
    {
        RoleGraphAnalysis roleAnalysis = this.roleGraph.Analyze(roleDefinitions);
        IReadOnlySet<string> effectiveRoles = this.roleGraph.Expand(request.Subject.Roles, roleAnalysis);
        List<PolicyTraceEntry> trace = [];

        IEnumerable<PolicyRule> orderedRules = policySet.Rules
            .OrderByDescending(static rule => rule.Priority)
            .ThenBy(static rule => rule.Effect == PolicyEffect.Deny ? 0 : 1)
            .ThenBy(static rule => rule.Id, StringComparer.OrdinalIgnoreCase);

        foreach (PolicyRule rule in orderedRules)
        {
            RuleMatch match = this.MatchRule(rule, request, effectiveRoles);
            trace.Add(new(rule.Id, rule.DisplayName, match.Matched, rule.Effect, rule.Priority, match.Explanation));
        }

        PolicyTraceEntry? highestMatch = trace.FirstOrDefault(static entry => entry.Matched);
        if (highestMatch is null)
        {
            return new(
                roleAnalysis.Diagnostics.Count > 0 ? PolicyDecisionKind.Indeterminate : PolicyDecisionKind.NotApplicable,
                "No policy rule matched the request.",
                trace,
                effectiveRoles,
                roleAnalysis.Diagnostics);
        }

        int winningPriority = highestMatch.Priority;
        PolicyTraceEntry[] winningMatches = trace.Where(entry => entry.Matched && entry.Priority == winningPriority).ToArray();
        bool denied = winningMatches.Any(static entry => entry.Effect == PolicyEffect.Deny);
        PolicyDecisionKind kind = denied ? PolicyDecisionKind.Deny : PolicyDecisionKind.Allow;
        string[] winningRuleNames = winningMatches
            .Where(entry => denied ? entry.Effect == PolicyEffect.Deny : entry.Effect == PolicyEffect.Allow)
            .Select(static entry => entry.RuleName)
            .ToArray();
        string summary = $"{kind} by {string.Join(", ", winningRuleNames)} at priority {winningPriority}.";
        return new(kind, summary, trace, effectiveRoles, roleAnalysis.Diagnostics);
    }

    private RuleMatch MatchRule(PolicyRule rule, PolicyRequest request, IReadOnlySet<string> effectiveRoles)
    {
        if (!rule.Enabled)
        {
            return new(false, "Rule is disabled.");
        }

        if (rule.Roles.Count > 0 && !rule.Roles.Overlaps(effectiveRoles))
        {
            return new(false, "No required role is assigned.");
        }

        if (!MatchesAny(rule.Actions, request.Action))
        {
            return new(false, $"Action '{request.Action}' does not match.");
        }

        if (!MatchesAny(rule.ResourceTypes, request.Resource.Type))
        {
            return new(false, $"Resource type '{request.Resource.Type}' does not match.");
        }

        List<string> explanations = [];
        foreach (PolicyCondition condition in rule.Conditions)
        {
            bool matched = this.conditionEvaluator.Evaluate(condition, request, out string explanation);
            explanations.Add(explanation);
            if (!matched)
            {
                return new(false, string.Join(" ", explanations));
            }
        }

        return new(true, explanations.Count == 0 ? "Role, action, and resource matched." : string.Join(" ", explanations));
    }

    private static bool MatchesAny(IReadOnlySet<string> patterns, string value)
    {
        return patterns.Count == 0 || patterns.Any(pattern => WildcardMatch(pattern, value));
    }

    private static bool WildcardMatch(string pattern, string value)
    {
        if (pattern == "*")
        {
            return true;
        }

        if (pattern.EndsWith('*'))
        {
            return value.StartsWith(pattern[..^1], StringComparison.OrdinalIgnoreCase);
        }

        return string.Equals(pattern, value, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record RuleMatch(bool Matched, string Explanation);
}
