namespace AtlasOps.Enterprise.Tests.Policy;

using AtlasOps.Enterprise.Contracts.Policy;
using AtlasOps.Enterprise.Core.Policy;
using AtlasOps.Enterprise.Core.Scenarios;

[TestClass]
public sealed class PolicyEngineTests
{
    [TestMethod]
    public void Evaluate_AdministratorDeletesProductionAsset_DenyTakesPrecedence()
    {
        PolicyRequest request = EnterpriseScenarioCatalog.CreatePolicyRequests()[1];
        PolicyEvaluator evaluator = new();

        PolicyDecision result = evaluator.Evaluate(
            request,
            EnterpriseScenarioCatalog.CreateOperationsPolicy(),
            EnterpriseScenarioCatalog.CreateRoles());

        Assert.AreEqual(PolicyDecisionKind.Deny, result.Kind);
        Assert.Contains("administrator", result.EffectiveRoles);
        Assert.Contains("viewer", result.EffectiveRoles);
        Assert.IsTrue(result.Trace.Any(static entry => entry.RuleId == "deny-production-delete" && entry.Matched));
    }

    [TestMethod]
    public void Evaluate_IncidentCommanderOnVpn_AllowsUpdate()
    {
        PolicyRequest request = EnterpriseScenarioCatalog.CreatePolicyRequests()[0];
        PolicyEvaluator evaluator = new();

        PolicyDecision result = evaluator.Evaluate(
            request,
            EnterpriseScenarioCatalog.CreateOperationsPolicy(),
            EnterpriseScenarioCatalog.CreateRoles());

        Assert.AreEqual(PolicyDecisionKind.Allow, result.Kind);
        Assert.IsTrue(result.Trace.Any(static entry => entry.RuleId == "allow-incident-update" && entry.Matched));
    }

    [TestMethod]
    public void Analyze_OverlappingAllowAndDeny_ReturnsDiagnostic()
    {
        PolicySet policy = EnterpriseScenarioCatalog.CreateOperationsPolicy();
        PolicyAnalyzer analyzer = new();

        IReadOnlyList<string> result = analyzer.Analyze(policy);

        Assert.IsNotEmpty(result);
        Assert.IsTrue(result.Any(static diagnostic => diagnostic.Contains("overlaps", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Analyze_CyclicRoleGraph_ReturnsCycleWithoutRecursingForever()
    {
        IReadOnlyList<RoleDefinition> roles =
        [
            new("a", "A", new HashSet<string>(["b"])),
            new("b", "B", new HashSet<string>(["a"])),
        ];
        RoleGraph graph = new();

        RoleGraphAnalysis result = graph.Analyze(roles);

        Assert.IsTrue(result.Diagnostics.Any(static diagnostic => diagnostic.Contains("cycle", StringComparison.OrdinalIgnoreCase)));
        Assert.Contains("a", result.EffectiveRoles["a"]);
    }
}
