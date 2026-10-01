namespace AtlasOps.Integrations.Identity.Core;

using AtlasOps.Integrations.Identity.Contracts;

public sealed class IdentityReconciliationService
{
    public IdentityReconciliationPlan CreatePlan(
        IdentitySubject current,
        IdentitySubject desired,
        IReadOnlyList<AccessGrant> currentGrants,
        IReadOnlyList<AccessGrant> desiredGrants,
        DateTimeOffset now)
    {
        List<string> diagnostics = [];
        if (!string.Equals(current.SubjectId, desired.SubjectId, StringComparison.Ordinal))
        {
            diagnostics.Add("Current and desired subjects must have the same provider identifier.");
            return new IdentityReconciliationPlan([], diagnostics);
        }

        List<IdentityChange> changes = [];
        if (current.Enabled && !desired.Enabled)
        {
            changes.Add(new IdentityChange(
                IdentityChangeKind.DisableSubject,
                current.SubjectId,
                string.Empty,
                null,
                0,
                "Desired identity is disabled."));
        }

        Dictionary<string, AccessGrant> desiredByKey = desiredGrants.ToDictionary(
            CreateGrantKey,
            StringComparer.OrdinalIgnoreCase);
        foreach (AccessGrant grant in currentGrants)
        {
            string key = CreateGrantKey(grant);
            bool expired = grant.ExpiresAt is not null && grant.ExpiresAt <= now;
            if (!desiredByKey.ContainsKey(key) || expired || !desired.Enabled)
            {
                changes.Add(new IdentityChange(
                    IdentityChangeKind.RevokeGrant,
                    current.SubjectId,
                    grant.Resource,
                    grant.Role,
                    10,
                    expired ? "Grant expired." : "Grant is absent from desired state."));
            }
        }

        if (!string.Equals(current.Revision, desired.Revision, StringComparison.Ordinal) ||
            !AttributesEqual(current.Attributes, desired.Attributes))
        {
            changes.Add(new IdentityChange(
                IdentityChangeKind.UpdateSubject,
                current.SubjectId,
                string.Empty,
                null,
                20,
                "Subject attributes or revision changed."));
        }

        HashSet<string> currentKeys = currentGrants
            .Select(CreateGrantKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (AccessGrant grant in desiredGrants)
        {
            if (!currentKeys.Contains(CreateGrantKey(grant)) &&
                (grant.ExpiresAt is null || grant.ExpiresAt > now))
            {
                changes.Add(new IdentityChange(
                    IdentityChangeKind.AddGrant,
                    current.SubjectId,
                    grant.Resource,
                    grant.Role,
                    30,
                    "Grant is required by desired state."));
            }
        }

        if (!current.Enabled && desired.Enabled)
        {
            changes.Add(new IdentityChange(
                IdentityChangeKind.EnableSubject,
                current.SubjectId,
                string.Empty,
                null,
                40,
                "Desired identity is enabled."));
        }

        IdentityChange[] ordered = changes
            .OrderBy(static change => change.Order)
            .ThenBy(static change => change.Resource, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static change => change.Role, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return new IdentityReconciliationPlan(ordered, diagnostics);
    }

    private static string CreateGrantKey(AccessGrant grant)
    {
        return $"{grant.Resource.Trim().ToUpperInvariant()}:{grant.Role.Trim().ToUpperInvariant()}";
    }

    private static bool AttributesEqual(
        IReadOnlyDictionary<string, string> left,
        IReadOnlyDictionary<string, string> right)
    {
        return left.Count == right.Count &&
               left.All(pair =>
                   right.TryGetValue(pair.Key, out string? value) &&
                   string.Equals(pair.Value, value, StringComparison.Ordinal));
    }
}
