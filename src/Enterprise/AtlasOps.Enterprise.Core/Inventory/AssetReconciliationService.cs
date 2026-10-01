namespace AtlasOps.Enterprise.Core.Inventory;

using AtlasOps.Enterprise.Contracts.Inventory;

public sealed class AssetReconciliationService
{
    public AssetReconciliationResult Reconcile(AssetRecord existing, IReadOnlyList<AssetEvidence> evidence)
    {
        List<string> diagnostics = [];
        List<ReconciledProperty> reconciled = [];
        HashSet<string> propertyNames = new(StringComparer.OrdinalIgnoreCase);
        propertyNames.UnionWith(existing.Properties.Keys);
        foreach (AssetEvidence item in evidence)
        {
            propertyNames.UnionWith(item.Properties.Keys);
        }

        foreach (string propertyName in propertyNames.Order(StringComparer.OrdinalIgnoreCase))
        {
            List<PropertyCandidate> candidates = [];
            if (existing.Properties.TryGetValue(propertyName, out string? existingValue))
            {
                candidates.Add(new(existingValue, AssetEvidenceSource.Manual, "canonical", existing.LastSeenAt, 0.85d));
            }

            foreach (AssetEvidence item in evidence)
            {
                if (item.Properties.TryGetValue(propertyName, out string? evidenceValue))
                {
                    double recency = CalculateRecency(item.ObservedAt, evidence);
                    double confidence = Math.Clamp(item.SourceReliability * 0.8d + recency * 0.2d, 0d, 1d);
                    candidates.Add(new(evidenceValue, item.Source, item.Id, item.ObservedAt, confidence));
                }
            }

            PropertyCandidate? winner = candidates
                .OrderByDescending(static candidate => candidate.Confidence)
                .ThenByDescending(static candidate => candidate.ObservedAt)
                .ThenBy(static candidate => candidate.EvidenceId, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
            if (winner is not null)
            {
                reconciled.Add(new(propertyName, winner.Value, winner.Source, winner.EvidenceId, winner.ObservedAt, winner.Confidence));
            }

            int distinctValues = candidates.Select(static candidate => candidate.Value).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            if (distinctValues > 1)
            {
                diagnostics.Add($"Property '{propertyName}' had {distinctValues} competing values.");
            }
        }

        Dictionary<string, string> properties = reconciled.ToDictionary(static property => property.Name, static property => property.Value, StringComparer.OrdinalIgnoreCase);
        DateTimeOffset lastSeen = evidence.Count == 0 ? existing.LastSeenAt : evidence.Max(static item => item.ObservedAt);
        AssetRecord updated = existing with
        {
            Properties = properties,
            LastSeenAt = lastSeen,
            LifecycleState = AssetLifecycleState.Active,
        };
        return new(updated, reconciled, diagnostics);
    }

    private static double CalculateRecency(DateTimeOffset observedAt, IReadOnlyList<AssetEvidence> evidence)
    {
        if (evidence.Count == 0)
        {
            return 0d;
        }

        DateTimeOffset newest = evidence.Max(static item => item.ObservedAt);
        double ageDays = Math.Max(0d, (newest - observedAt).TotalDays);
        return 1d / (1d + ageDays / 30d);
    }

    private sealed record PropertyCandidate(
        string Value,
        AssetEvidenceSource Source,
        string EvidenceId,
        DateTimeOffset ObservedAt,
        double Confidence);
}
