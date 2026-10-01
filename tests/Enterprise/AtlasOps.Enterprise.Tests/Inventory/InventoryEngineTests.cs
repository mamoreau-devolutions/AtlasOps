namespace AtlasOps.Enterprise.Tests.Inventory;

using AtlasOps.Enterprise.Contracts.Inventory;
using AtlasOps.Enterprise.Core.Inventory;
using AtlasOps.Enterprise.Core.Scenarios;

[TestClass]
public sealed class InventoryEngineTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-10-01T12:00:00Z");

    [TestMethod]
    public void Match_ImmutableIdentifierMatch_ReturnsConfidentCandidate()
    {
        AssetRecord asset = EnterpriseScenarioCatalog.CreateAssets(Now)[0];
        AssetEvidence evidence = EnterpriseScenarioCatalog.CreateWebServerEvidence(Now);
        AssetMatcher matcher = new();

        AssetMatchResult result = matcher.Match(asset, evidence);

        Assert.IsTrue(result.IsConfidentMatch);
        Assert.IsInRange(70d, 100d, result.Score);
        Assert.IsTrue(result.Reasons.Any(static reason => reason.Kind == "identifier:cloud-resource-id" && reason.Matched));
    }

    [TestMethod]
    public void Detect_MissingFirewall_ReturnsCriticalDrift()
    {
        AssetRecord asset = EnterpriseScenarioCatalog.CreateAssets(Now)[0];
        DesiredAssetState desired = EnterpriseScenarioCatalog.CreateWebServerDesiredState();
        AssetDriftDetector detector = new();

        IReadOnlyList<AssetDrift> result = detector.Detect(asset, desired);

        AssetDrift firewall = Assert.ContainsSingle(result.Where(static drift => drift.Property == "firewall"));
        Assert.AreEqual(DriftKind.Missing, firewall.Kind);
        Assert.AreEqual(DriftSeverity.Critical, firewall.Severity);
    }

    [TestMethod]
    public void Reconcile_NewerReliableEvidence_UpdatesCanonicalProperties()
    {
        AssetRecord asset = EnterpriseScenarioCatalog.CreateAssets(Now)[0];
        AssetEvidence evidence = EnterpriseScenarioCatalog.CreateWebServerEvidence(Now);
        AssetReconciliationService service = new();

        AssetReconciliationResult result = service.Reconcile(asset, [evidence]);

        Assert.AreEqual("Ubuntu 26.04", result.Asset.Properties["operatingSystem"]);
        Assert.AreEqual("6.14", result.Asset.Properties["kernel"]);
        Assert.IsTrue(result.Diagnostics.Any(static diagnostic => diagnostic.Contains("competing", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Traverse_CyclicRelationships_VisitsEachAssetOnce()
    {
        IReadOnlyList<AssetRecord> assets = EnterpriseScenarioCatalog.CreateAssets(Now);
        IReadOnlyList<AssetRelationship> relationships =
        [
            new(assets[0].Id, assets[1].Id, "depends-on", true),
            new(assets[1].Id, assets[2].Id, "feeds", false),
            new(assets[2].Id, assets[0].Id, "reports-to", false),
        ];
        AssetTopologyService service = new();

        IReadOnlyList<AssetTopologyNode> result = service.Traverse(assets[0].Id, assets, relationships);

        Assert.HasCount(2, result);
        Assert.AreEqual(assets[1].Id, result[0].Asset.Id);
        Assert.AreEqual(assets[2].Id, result[1].Asset.Id);
    }
}
