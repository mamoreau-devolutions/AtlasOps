namespace AtlasOps.Enterprise.Core.Inventory;

using AtlasOps.Enterprise.Contracts.Inventory;

public sealed class AssetMatcher
{
    public AssetMatchResult Match(AssetRecord asset, AssetEvidence evidence, double confidenceThreshold = 70d)
    {
        List<AssetMatchReason> reasons = [];
        double score = 0d;

        foreach (AssetIdentifier evidenceIdentifier in evidence.Identifiers)
        {
            AssetIdentifier? matchingIdentifier = asset.Identifiers.FirstOrDefault(
                identifier =>
                    string.Equals(identifier.Scheme, evidenceIdentifier.Scheme, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(identifier.Value, evidenceIdentifier.Value, StringComparison.OrdinalIgnoreCase));
            if (matchingIdentifier is null)
            {
                reasons.Add(new($"identifier:{evidenceIdentifier.Scheme}", $"Identifier {evidenceIdentifier.Scheme} did not match.", 0d, false));
                continue;
            }

            double identifierWeight = matchingIdentifier.IsImmutable || evidenceIdentifier.IsImmutable ? 65d : 35d;
            score += identifierWeight;
            reasons.Add(new($"identifier:{evidenceIdentifier.Scheme}", $"Identifier {evidenceIdentifier.Scheme} matched.", identifierWeight, true));
        }

        bool nameMatched = string.Equals(asset.DisplayName, evidence.DisplayName, StringComparison.OrdinalIgnoreCase);
        double nameSimilarity = CalculateSimilarity(asset.DisplayName, evidence.DisplayName);
        double nameWeight = nameMatched ? 20d : Math.Round(nameSimilarity * 12d, 2);
        score += nameWeight;
        reasons.Add(new("name", nameMatched ? "Display names matched exactly." : $"Display names were {nameSimilarity:P0} similar.", nameWeight, nameWeight > 0d));

        bool typeMatched = string.Equals(asset.AssetType, evidence.AssetType, StringComparison.OrdinalIgnoreCase);
        double typeWeight = typeMatched ? 15d : -20d;
        score += typeWeight;
        reasons.Add(new("type", typeMatched ? "Asset types matched." : "Asset types differed.", typeWeight, typeMatched));

        score = Math.Clamp(score * Math.Clamp(evidence.SourceReliability, 0.1d, 1d), 0d, 100d);
        return new(asset.Id, evidence.Id, Math.Round(score, 2), score >= confidenceThreshold, reasons);
    }

    public IReadOnlyList<AssetMatchResult> RankCandidates(
        IReadOnlyList<AssetRecord> assets,
        AssetEvidence evidence,
        double confidenceThreshold = 70d)
    {
        return assets
            .Select(asset => this.Match(asset, evidence, confidenceThreshold))
            .OrderByDescending(static result => result.Score)
            .ThenBy(static result => result.AssetId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static double CalculateSimilarity(string left, string right)
    {
        if (left.Length == 0 || right.Length == 0)
        {
            return 0d;
        }

        string normalizedLeft = left.Trim().ToUpperInvariant();
        string normalizedRight = right.Trim().ToUpperInvariant();
        int[,] distance = new int[normalizedLeft.Length + 1, normalizedRight.Length + 1];

        for (int leftIndex = 0; leftIndex <= normalizedLeft.Length; leftIndex++)
        {
            distance[leftIndex, 0] = leftIndex;
        }

        for (int rightIndex = 0; rightIndex <= normalizedRight.Length; rightIndex++)
        {
            distance[0, rightIndex] = rightIndex;
        }

        for (int leftIndex = 1; leftIndex <= normalizedLeft.Length; leftIndex++)
        {
            for (int rightIndex = 1; rightIndex <= normalizedRight.Length; rightIndex++)
            {
                int substitutionCost = normalizedLeft[leftIndex - 1] == normalizedRight[rightIndex - 1] ? 0 : 1;
                distance[leftIndex, rightIndex] = Math.Min(
                    Math.Min(distance[leftIndex - 1, rightIndex] + 1, distance[leftIndex, rightIndex - 1] + 1),
                    distance[leftIndex - 1, rightIndex - 1] + substitutionCost);
            }
        }

        int maximumLength = Math.Max(normalizedLeft.Length, normalizedRight.Length);
        return 1d - (double)distance[normalizedLeft.Length, normalizedRight.Length] / maximumLength;
    }
}
