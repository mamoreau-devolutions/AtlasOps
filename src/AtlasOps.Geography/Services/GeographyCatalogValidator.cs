namespace AtlasOps.Geography.Services;

using AtlasOps.Geography.Domain;

public sealed class GeographyCatalogValidator
{
    public GeographyValidationReport Validate(GeographyCatalog catalog)
    {
        List<GeographyValidationIssue> issues = [];
        AddDuplicateIssues(
            catalog.Countries,
            static country => country.Alpha2,
            "unique-country-alpha2",
            issues);
        AddDuplicateIssues(
            catalog.Countries.Where(static country => !string.IsNullOrWhiteSpace(country.Alpha3)),
            static country => country.Alpha3,
            "unique-country-alpha3",
            issues);
        AddDuplicateIssues(
            catalog.Subdivisions,
            static subdivision => subdivision.Id,
            "unique-subdivision-id",
            issues);

        HashSet<string> countries = catalog.Countries
            .Select(static country => country.Alpha2)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (AdministrativeDivision subdivision in catalog.Subdivisions)
        {
            if (!countries.Contains(subdivision.CountryCode))
            {
                issues.Add(new GeographyValidationIssue(
                    GeographyValidationSeverity.Error,
                    "subdivision-country-reference",
                    subdivision.Id,
                    $"Subdivision references unknown country '{subdivision.CountryCode}'."));
            }

            ValidateCoordinate(subdivision.Id, subdivision.Coordinate, issues);
        }

        foreach (Country country in catalog.Countries)
        {
            ValidateCoordinate(country.Alpha2, country.Coordinate, issues);
            if (country.Alpha2.Length != 2)
            {
                issues.Add(new GeographyValidationIssue(
                    GeographyValidationSeverity.Error,
                    "country-alpha2-format",
                    country.Alpha2,
                    "Country alpha-2 code must contain two characters."));
            }
        }

        return new GeographyValidationReport(issues);
    }

    private static void AddDuplicateIssues<T>(
        IEnumerable<T> records,
        Func<T, string> keySelector,
        string rule,
        ICollection<GeographyValidationIssue> issues)
    {
        IEnumerable<IGrouping<string, T>> duplicates = records
            .GroupBy(keySelector, StringComparer.OrdinalIgnoreCase)
            .Where(static group => group.Count() > 1);
        foreach (IGrouping<string, T> duplicate in duplicates)
        {
            issues.Add(new GeographyValidationIssue(
                GeographyValidationSeverity.Error,
                rule,
                duplicate.Key,
                $"Identifier '{duplicate.Key}' occurs {duplicate.Count()} times."));
        }
    }

    private static void ValidateCoordinate(
        string recordId,
        GeoCoordinate? coordinate,
        ICollection<GeographyValidationIssue> issues)
    {
        if (coordinate is null)
        {
            return;
        }

        if (coordinate.Latitude is < -90 or > 90 || coordinate.Longitude is < -180 or > 180)
        {
            issues.Add(new GeographyValidationIssue(
                GeographyValidationSeverity.Error,
                "coordinate-range",
                recordId,
                "Latitude or longitude is outside the valid range."));
        }
    }
}