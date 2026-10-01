namespace AtlasOps.Integrations.GitProviders.Core;

using AtlasOps.Integrations.GitProviders.Contracts;

public sealed class GitProviderService
{
    public BranchPolicyEvaluation Evaluate(
        PullRequestReference pullRequest,
        BranchPolicy policy,
        int approvals,
        IReadOnlyDictionary<string, bool> checks,
        bool hasMergeCommit,
        bool forcePushRequested)
    {
        List<string> diagnostics = [];

        if (pullRequest.Draft)
        {
            diagnostics.Add("Draft pull requests cannot be merged.");
        }

        if (approvals < policy.MinimumApprovals)
        {
            diagnostics.Add(
                $"At least {policy.MinimumApprovals} approvals are required; {approvals} were supplied.");
        }

        if (policy.RequireLinearHistory && hasMergeCommit)
        {
            diagnostics.Add("The branch policy requires linear history.");
        }

        if (!policy.AllowForcePush && forcePushRequested)
        {
            diagnostics.Add("Force push is not allowed by the branch policy.");
        }

        foreach (string requiredCheck in policy.RequiredChecks.Order(StringComparer.OrdinalIgnoreCase))
        {
            if (!checks.TryGetValue(requiredCheck, out bool passed) || !passed)
            {
                diagnostics.Add($"Required check '{requiredCheck}' has not passed.");
            }
        }

        if (policy.RequirePassingChecks && checks.Values.Any(static passed => !passed))
        {
            diagnostics.Add("One or more reported checks have failed.");
        }

        return new BranchPolicyEvaluation(diagnostics.Count == 0, diagnostics);
    }

    public GitSynchronizationDelta CreateDelta(
        IReadOnlyList<PullRequestReference> previous,
        IReadOnlyList<PullRequestReference> current,
        string nextCursor)
    {
        Dictionary<long, PullRequestReference> previousByNumber = previous.ToDictionary(
            static item => item.Number);
        Dictionary<long, PullRequestReference> currentByNumber = current.ToDictionary(
            static item => item.Number);

        PullRequestReference[] added = current
            .Where(item => !previousByNumber.ContainsKey(item.Number))
            .OrderBy(static item => item.Number)
            .ToArray();
        PullRequestReference[] updated = current
            .Where(item =>
                previousByNumber.TryGetValue(item.Number, out PullRequestReference? old) &&
                !string.Equals(old.Revision, item.Revision, StringComparison.Ordinal))
            .OrderBy(static item => item.Number)
            .ToArray();
        long[] removed = previous
            .Where(item => !currentByNumber.ContainsKey(item.Number))
            .Select(static item => item.Number)
            .Order()
            .ToArray();

        return new GitSynchronizationDelta(added, updated, removed, nextCursor);
    }

    public IReadOnlyList<PullRequestReference> ReadPage(
        IReadOnlyList<PullRequestReference> source,
        string? cursor,
        int pageSize)
    {
        if (pageSize is < 1 or > 500)
        {
            return [];
        }

        long after = 0;
        if (!string.IsNullOrWhiteSpace(cursor) &&
            !long.TryParse(cursor, out after))
        {
            return [];
        }

        return source
            .Where(item => item.Number > after)
            .OrderBy(static item => item.Number)
            .Take(pageSize)
            .ToArray();
    }
}
