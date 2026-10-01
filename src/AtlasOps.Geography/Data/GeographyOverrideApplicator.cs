namespace AtlasOps.Geography.Data;

using AtlasOps.Geography.Domain;

public static class GeographyOverrideApplicator
{
    public static GeographyCatalog Apply(GeographyCatalog catalog)
    {
        Dictionary<string, CountryOverride> countryOverrides = catalog.Overrides.Countries
            .ToDictionary(static item => item.CountryCode, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, SubdivisionOverride> subdivisionOverrides = catalog.Overrides.Subdivisions
            .ToDictionary(static item => item.SubdivisionId, StringComparer.OrdinalIgnoreCase);

        IReadOnlyList<Country> countries = catalog.Countries
            .Select(country => ApplyCountry(country, countryOverrides.GetValueOrDefault(country.Alpha2)))
            .ToArray();
        IReadOnlyList<AdministrativeDivision> subdivisions = catalog.Subdivisions
            .Select(subdivision => ApplySubdivision(
                subdivision,
                subdivisionOverrides.GetValueOrDefault(subdivision.Id)))
            .ToArray();
        return catalog with
        {
            Countries = countries,
            Subdivisions = subdivisions,
        };
    }

    private static Country ApplyCountry(Country country, CountryOverride? item)
    {
        return item is null
            ? country
            : country with
            {
                Name = Coalesce(item.Name, country.Name),
                OfficialName = Coalesce(item.OfficialName, country.OfficialName),
                CurrencyCode = Coalesce(item.CurrencyCode, country.CurrencyCode),
                CallingCode = Coalesce(item.CallingCode, country.CallingCode),
            };
    }

    private static AdministrativeDivision ApplySubdivision(
        AdministrativeDivision subdivision,
        SubdivisionOverride? item)
    {
        return item is null
            ? subdivision
            : subdivision with
            {
                Name = Coalesce(item.Name, subdivision.Name),
                Type = Coalesce(item.Type, subdivision.Type),
            };
    }

    private static string Coalesce(string? candidate, string fallback)
    {
        return string.IsNullOrWhiteSpace(candidate) ? fallback : candidate.Trim();
    }
}