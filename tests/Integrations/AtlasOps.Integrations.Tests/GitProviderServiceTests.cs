namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.GitProviders.Contracts;
using AtlasOps.Integrations.GitProviders.Core;

[TestClass]
public sealed class GitProviderServiceTests
{
    [TestMethod]
    public void Evaluate_AllPolicyViolations_ReturnsDeterministicallyOrderedDiagnostics()
    {
        GitProviderService service = new();
        PullRequestReference pullRequest = PullRequest(1, "1", draft: true);
        BranchPolicy policy = new(
            2,
            true,
            true,
            false,
            new HashSet<string>(["z-check", "a-check"], StringComparer.OrdinalIgnoreCase));
        Dictionary<string, bool> checks = new()
        {
            ["extra"] = false,
            ["z-check"] = false,
        };

        BranchPolicyEvaluation result = service.Evaluate(pullRequest, policy, 1, checks, true, true);

        Assert.IsFalse(result.Allowed);
        CollectionAssert.AreEqual(
            new[]
            {
                "Draft pull requests cannot be merged.",
                "At least 2 approvals are required; 1 were supplied.",
                "The branch policy requires linear history.",
                "Force push is not allowed by the branch policy.",
                "Required check 'a-check' has not passed.",
                "Required check 'z-check' has not passed.",
                "One or more reported checks have failed.",
            },
            result.Diagnostics.ToArray());
    }

    [TestMethod]
    public void Evaluate_SatisfiedPolicy_IsAllowed()
    {
        GitProviderService service = new();
        BranchPolicy policy = new(1, true, false, true, new HashSet<string>(["build"]));

        BranchPolicyEvaluation result = service.Evaluate(
            PullRequest(1, "1"),
            policy,
            1,
            new Dictionary<string, bool> { ["build"] = true },
            false,
            true);

        Assert.IsTrue(result.Allowed);
        Assert.IsEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void CreateDelta_ClassifiesAndOrdersAddedUpdatedAndRemoved()
    {
        GitProviderService service = new();

        GitSynchronizationDelta result = service.CreateDelta(
            [PullRequest(3, "old"), PullRequest(1, "same"), PullRequest(2, "gone")],
            [PullRequest(4, "new"), PullRequest(3, "changed"), PullRequest(1, "same")],
            "cursor");

        CollectionAssert.AreEqual(new long[] { 4 }, result.Added.Select(static item => item.Number).ToArray());
        CollectionAssert.AreEqual(new long[] { 3 }, result.Updated.Select(static item => item.Number).ToArray());
        CollectionAssert.AreEqual(new long[] { 2 }, result.Removed.ToArray());
        Assert.AreEqual("cursor", result.NextCursor);
    }

    [TestMethod]
    public void ReadPage_ValidCursorIsExclusiveAndResultIsSorted()
    {
        GitProviderService service = new();

        IReadOnlyList<PullRequestReference> result = service.ReadPage(
            [PullRequest(3, "3"), PullRequest(1, "1"), PullRequest(2, "2")],
            "1",
            2);

        CollectionAssert.AreEqual(new long[] { 2, 3 }, result.Select(static item => item.Number).ToArray());
    }

    [TestMethod]
    public void ReadPage_InvalidCursorOrPageSizeBoundary_ReturnsEmpty()
    {
        GitProviderService service = new();
        IReadOnlyList<PullRequestReference> source = [PullRequest(1, "1")];

        Assert.IsEmpty(service.ReadPage(source, "not-a-number", 1));
        Assert.IsEmpty(service.ReadPage(source, null, 0));
        Assert.IsEmpty(service.ReadPage(source, null, 501));
        Assert.HasCount(1, service.ReadPage(source, null, 500));
    }

    private static PullRequestReference PullRequest(long number, string revision, bool draft = false)
    {
        return new PullRequestReference("provider", "repo", number, "source", "target", revision, draft, []);
    }
}
