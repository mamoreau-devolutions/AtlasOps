namespace AtlasOps.Enterprise.Core.Inventory;

using AtlasOps.Enterprise.Contracts.Inventory;

public sealed class AssetTopologyService
{
    public IReadOnlyList<AssetTopologyNode> Traverse(
        string rootAssetId,
        IReadOnlyList<AssetRecord> assets,
        IReadOnlyList<AssetRelationship> relationships,
        int maximumDepth = 8)
    {
        Dictionary<string, AssetRecord> assetsById = assets.ToDictionary(static asset => asset.Id, StringComparer.OrdinalIgnoreCase);
        ILookup<string, AssetRelationship> outgoing = relationships.ToLookup(static relationship => relationship.SourceAssetId, StringComparer.OrdinalIgnoreCase);
        Queue<(string Id, int Distance, IReadOnlyList<string> Path)> pending = new();
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase) { rootAssetId };
        List<AssetTopologyNode> nodes = [];
        pending.Enqueue((rootAssetId, 0, [rootAssetId]));

        while (pending.TryDequeue(out (string Id, int Distance, IReadOnlyList<string> Path) current))
        {
            if (current.Distance >= maximumDepth)
            {
                continue;
            }

            foreach (AssetRelationship relationship in outgoing[current.Id]
                         .OrderBy(static relationship => relationship.TargetAssetId, StringComparer.OrdinalIgnoreCase))
            {
                if (!visited.Add(relationship.TargetAssetId) || !assetsById.TryGetValue(relationship.TargetAssetId, out AssetRecord? asset))
                {
                    continue;
                }

                string[] path = [.. current.Path, asset.Id];
                AssetTopologyNode node = new(asset, current.Distance + 1, path);
                nodes.Add(node);
                pending.Enqueue((asset.Id, current.Distance + 1, path));
            }
        }

        return nodes;
    }
}
