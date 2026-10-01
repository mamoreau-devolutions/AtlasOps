namespace AtlasOps.ReferenceData.Infrastructure;

public sealed class ReferenceSearchIndex
{
    private readonly IReadOnlyList<ReferenceDataRow> rows;
    private readonly Dictionary<string, HashSet<int>> tokens = new(StringComparer.Ordinal);

    public ReferenceSearchIndex(IReadOnlyList<ReferenceDataRow> rows)
    {
        this.rows = rows;
        for (int index = 0; index < rows.Count; index++)
        {
            foreach (string token in Tokenize(rows[index].SearchText))
            {
                if (!this.tokens.TryGetValue(token, out HashSet<int>? matches))
                {
                    matches = [];
                    this.tokens[token] = matches;
                }

                matches.Add(index);
            }
        }
    }

    public IReadOnlyList<ReferenceDataRow> Search(string query, int limit = 500)
    {
        string[] queryTokens = Tokenize(query).Distinct(StringComparer.Ordinal).ToArray();
        if (queryTokens.Length == 0)
        {
            return this.rows.Take(limit).ToArray();
        }

        Dictionary<int, int> scores = [];
        foreach (string token in queryTokens)
        {
            foreach (KeyValuePair<string, HashSet<int>> entry in this.tokens)
            {
                if (!entry.Key.StartsWith(token, StringComparison.Ordinal) &&
                    !entry.Key.Contains(token, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (int index in entry.Value)
                {
                    scores[index] = scores.GetValueOrDefault(index) + (entry.Key == token ? 4 : 1);
                }
            }
        }

        return scores
            .OrderByDescending(static item => item.Value)
            .ThenBy(static item => item.Key)
            .Take(limit)
            .Select(item => this.rows[item.Key])
            .ToArray();
    }

    private static IEnumerable<string> Tokenize(string value)
    {
        return value
            .ToLowerInvariant()
            .Split(
                [' ', '\t', '\r', '\n', '-', '_', '/', '\\', '.', ',', ':', ';', '(', ')', '[', ']'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}