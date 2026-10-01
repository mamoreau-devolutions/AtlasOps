namespace AtlasOps.Enterprise.Contracts.Policy;

public enum PolicyEffect
{
    Allow,
    Deny,
}

public enum PolicyDecisionKind
{
    Allow,
    Deny,
    NotApplicable,
    Indeterminate,
}

public enum PolicyConditionOperator
{
    Equals,
    NotEquals,
    Contains,
    StartsWith,
    EndsWith,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Exists,
    In,
}

public sealed record PolicySubject(
    string Id,
    IReadOnlySet<string> Roles,
    IReadOnlyDictionary<string, string> Attributes);

public sealed record PolicyResource(
    string Id,
    string Type,
    IReadOnlyDictionary<string, string> Attributes);

public sealed record PolicyRequest(
    PolicySubject Subject,
    string Action,
    PolicyResource Resource,
    IReadOnlyDictionary<string, string> Environment);

public sealed record PolicyCondition(
    string Source,
    string Key,
    PolicyConditionOperator Operator,
    string? ExpectedValue = null);

public sealed record PolicyRule(
    string Id,
    string DisplayName,
    PolicyEffect Effect,
    int Priority,
    IReadOnlySet<string> Roles,
    IReadOnlySet<string> Actions,
    IReadOnlySet<string> ResourceTypes,
    IReadOnlyList<PolicyCondition> Conditions,
    bool Enabled = true);

public sealed record PolicySet(
    string Id,
    string DisplayName,
    IReadOnlyList<PolicyRule> Rules);

public sealed record RoleDefinition(
    string Id,
    string DisplayName,
    IReadOnlySet<string> InheritedRoleIds);

public sealed record PolicyTraceEntry(
    string RuleId,
    string RuleName,
    bool Matched,
    PolicyEffect Effect,
    int Priority,
    string Explanation);

public sealed record PolicyDecision(
    PolicyDecisionKind Kind,
    string Summary,
    IReadOnlyList<PolicyTraceEntry> Trace,
    IReadOnlySet<string> EffectiveRoles,
    IReadOnlyList<string> Diagnostics);

public sealed record RoleGraphAnalysis(
    IReadOnlyDictionary<string, IReadOnlySet<string>> EffectiveRoles,
    IReadOnlyList<string> Diagnostics);
