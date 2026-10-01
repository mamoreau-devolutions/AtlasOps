namespace AtlasOps.Product.Tests;

using AtlasOps.Product.Release;

[TestClass]
public sealed class ReleaseChannelServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 9, 10, 11, 12, TimeSpan.Zero);

    [TestMethod]
    public void Select_ChannelPrereleaseAndAgePolicyChoosesNewestEligibleArtifact()
    {
        ReleaseChannelService service = new();
        UpdatePolicy policy = new(ReleaseChannel.Preview, false, true, TimeSpan.FromDays(1));
        ReleaseArtifact[] artifacts =
        [
            Artifact("3.0", ReleaseChannel.Canary, Now.AddDays(-2)),
            Artifact("2.0", ReleaseChannel.Preview, Now.AddHours(-23)),
            Artifact("1.5", ReleaseChannel.Preview, Now.AddDays(-1)),
            Artifact("1.4", ReleaseChannel.Stable, Now.AddDays(-2)),
        ];

        UpdateDecision result = service.Select(new Version(1, 0), policy, artifacts, Now);

        Assert.IsTrue(result.Update);
        Assert.AreEqual(new Version(1, 5), result.Artifact!.Version);
        Assert.AreEqual(ReleaseChannel.Preview, result.Artifact.Channel);
    }

    [TestMethod]
    public void Select_MandatoryArtifactBypassesMinimumAgeAndReportsMandatoryUpgradePath()
    {
        ReleaseChannelService service = new();
        ReleaseArtifact mandatory = Artifact("5.0", ReleaseChannel.Stable, Now) with
        {
            Mandatory = true,
            MinimumSupportedVersion = new Version(4, 0),
        };

        UpdateDecision result = service.Select(
            new Version(1, 0),
            new UpdatePolicy(ReleaseChannel.Stable, false, false, TimeSpan.FromDays(30)),
            [mandatory],
            Now);

        Assert.IsTrue(result.Update);
        Assert.AreSame(mandatory, result.Artifact);
        CollectionAssert.AreEqual(
            new[] { "Installed version requires a mandatory upgrade path." },
            result.Diagnostics.ToArray());
    }

    [TestMethod]
    public void Select_DowngradeCurrentAndNoCandidate_ReturnDistinctDecisions()
    {
        ReleaseChannelService service = new();
        ReleaseArtifact old = Artifact("1.0", ReleaseChannel.Stable, Now.AddDays(-2));
        UpdatePolicy policy = new(ReleaseChannel.Stable, false, false, TimeSpan.Zero);

        UpdateDecision downgrade = service.Select(new Version(2, 0), policy, [old], Now);
        UpdateDecision current = service.Select(new Version(1, 0), policy, [old], Now);
        UpdateDecision none = service.Select(
            new Version(1, 0),
            policy,
            [Artifact("2.0", ReleaseChannel.Preview, Now.AddDays(-2))],
            Now);

        Assert.IsFalse(downgrade.Update);
        Assert.AreEqual("Selected release is older than the installed version.", downgrade.Diagnostics.Single());
        Assert.IsFalse(current.Update);
        Assert.AreEqual("The installed version is current.", current.Diagnostics.Single());
        Assert.IsFalse(none.Update);
        Assert.IsNull(none.Artifact);
        Assert.AreEqual("No release satisfies the configured update policy.", none.Diagnostics.Single());
    }

    [TestMethod]
    public void Select_StablePolicyRejectsPreviewWhileCanaryPolicyAcceptsAllChannels()
    {
        ReleaseChannelService service = new();
        ReleaseArtifact preview = Artifact("2.0", ReleaseChannel.Preview, Now.AddDays(-1));

        UpdateDecision stable = service.Select(
            new Version(1, 0),
            new UpdatePolicy(ReleaseChannel.Stable, false, true, TimeSpan.Zero),
            [preview],
            Now);
        UpdateDecision canary = service.Select(
            new Version(1, 0),
            new UpdatePolicy(ReleaseChannel.Canary, false, true, TimeSpan.Zero),
            [preview],
            Now);

        Assert.IsFalse(stable.Update);
        Assert.IsTrue(canary.Update);
        Assert.AreSame(preview, canary.Artifact);
    }

    [TestMethod]
    public void Select_AllowDowngradeReturnsOlderEligibleArtifact()
    {
        ReleaseChannelService service = new();
        ReleaseArtifact old = Artifact("1.0", ReleaseChannel.Stable, Now.AddDays(-1));

        UpdateDecision result = service.Select(
            new Version(2, 0),
            new UpdatePolicy(ReleaseChannel.Stable, true, false, TimeSpan.Zero),
            [old],
            Now);

        Assert.IsTrue(result.Update);
        Assert.AreSame(old, result.Artifact);
        Assert.IsEmpty(result.Diagnostics);
    }

    private static ReleaseArtifact Artifact(
        string version,
        ReleaseChannel channel,
        DateTimeOffset publishedAt)
    {
        return new ReleaseArtifact(
            Version.Parse(version),
            channel,
            new Uri("https://example.invalid/download"),
            "HASH",
            publishedAt,
            new Version(1, 0),
            false);
    }
}
