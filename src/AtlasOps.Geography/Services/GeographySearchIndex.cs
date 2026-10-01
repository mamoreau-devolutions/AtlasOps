namespace AtlasOps.Geography.Services;

using System.Globalization;
using System.Text;

using AtlasOps.Geography.Domain;

public sealed class GeographySearchIndex
{
    private readonly Dictionary<string, HashSet<string>> trigrams = new(StringComparer.Ordinal);
    private readonly Dictionary<string, GeographySearchResult> records = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> searchableText = new(StringComparer.OrdinalIgnoreCase);

    public GeographySearchIndex(GeographyCatalog catalog)
    {
        foreach (Country country in catalog.Countries)
        {
            string id = $"country:{country.Alpha2}";
            string text = string.Join(
                ' ',
                country.Name,
                country.OfficialName,
                country.Alpha2,
                country.Alpha3,
                country.NumericCode,
                country.Continent,
                country.Region,
                country.Subregion,
                country.CurrencyCode,
                country.CallingCode,
                string.Join(' ', country.Aliases));
            this.Add(
                id,
                text,
                new GeographySearchResult(
                    GeographySearchResultKind.Country,
                    country.Alpha2,
                    country.Name,
                    $"{country.Alpha2} · {country.Continent} · {country.CurrencyCode}",
                    0));
        }

        foreach (AdministrativeDivision subdivision in catalog.Subdivisions)
        {
            string id = $"subdivision:{subdivision.Id}";
            string text = string.Join(
                ' ',
                subdivision.Name,
                subdivision.Code,
                subdivision.CountryCode,
                subdivision.Type,
                string.Join(' ', subdivision.Aliases));
            this.Add(
                id,
                text,
                new GeographySearchResult(
                    GeographySearchResultKind.Subdivision,
                    subdivision.Id,
                    subdivision.Name,
                    $"{subdivision.CountryCode} · {subdivision.Code} · {subdivision.Type}",
                    0));
        }
    }

    public IReadOnlyList<GeographySearchResult> Search(string query, int limit = 100)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return this.records.Values
                .Where(static result => result.Kind == GeographySearchResultKind.Country)
                .OrderBy(static result => result.PrimaryText, StringComparer.OrdinalIgnoreCase)
                .Take(limit)
                .ToArray();
        }

        string normalizedQuery = Normalize(query);
        IReadOnlyList<string> queryTrigrams = CreateTrigrams(normalizedQuery);
        IEnumerable<string> candidates = this.GetCandidates(queryTrigrams);
        return candidates
            .Select(id => this.Score(id, normalizedQuery, queryTrigrams))
            .Where(static result => result.Score > 0)
            .OrderByDescending(static result => result.Score)
            .ThenBy(static result => result.PrimaryText, StringComparer.OrdinalIgnoreCase)
            .Take(limit)
            .ToArray();
    }

    private void Add(string id, string text, GeographySearchResult result)
    {
        string normalized = Normalize(text);
        this.records[id] = result;
        this.searchableText[id] = normalized;
        foreach (string trigram in CreateTrigrams(normalized))
        {
            if (!this.trigrams.TryGetValue(trigram, out HashSet<string>? ids))
            {
                ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                this.trigrams.Add(trigram, ids);
            }

            ids.Add(id);
        }
    }

    private IEnumerable<string> GetCandidates(IReadOnlyList<string> queryTrigrams)
    {
        if (queryTrigrams.Count == 0)
        {
            return this.records.Keys;
        }

        HashSet<string>? candidates = null;
        foreach (string trigram in queryTrigrams)
        {
            if (!this.trigrams.TryGetValue(trigram, out HashSet<string>? matches))
            {
                continue;
            }

            if (candidates is null)
            {
                candidates = new HashSet<string>(matches, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                candidates.UnionWith(matches);
            }
        }

        return candidates ?? [];
    }

    private GeographySearchResult Score(
        string id,
        string normalizedQuery,
        IReadOnlyList<string> queryTrigrams)
    {
        string text = this.searchableText[id];
        int matchedTrigrams = queryTrigrams.Count(trigram => text.Contains(trigram, StringComparison.Ordinal));
        double trigramScore = queryTrigrams.Count == 0 ? 0 : (double)matchedTrigrams / queryTrigrams.Count;
        double phraseBonus = text.Contains(normalizedQuery, StringComparison.Ordinal) ? 2 : 0;
        double prefixBonus = text.StartsWith(normalizedQuery, StringComparison.Ordinal) ? 1 : 0;
        return this.records[id] with
        {
            Score = phraseBonus + prefixBonus + trigramScore,
        };
    }

    private static IReadOnlyList<string> CreateTrigrams(string text)
    {
        if (text.Length < 3)
        {
            return string.IsNullOrWhiteSpace(text) ? [] : [text];
        }

        HashSet<string> values = new(StringComparer.Ordinal);
        for (int index = 0; index <= text.Length - 3; index++)
        {
            values.Add(text.Substring(index, 3));
        }

        return values.ToArray();
    }

    private static string Normalize(string value)
    {
        string decomposed = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        StringBuilder builder = new(decomposed.Length);
        foreach (char character in decomposed)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(char.IsLetterOrDigit(character) ? character : ' ');
        }

        return string.Join(' ', builder.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}