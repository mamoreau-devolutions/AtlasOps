namespace AtlasOps.Geography.Domain;

public sealed record GeoCoordinate(double Latitude, double Longitude);

public sealed record Country(
    string Alpha2,
    string Alpha3,
    string NumericCode,
    string Name,
    string OfficialName,
    string Continent,
    string Region,
    string Subregion,
    string CurrencyCode,
    string CallingCode,
    string Nationality,
    string StartOfWeek,
    string DistanceUnit,
    bool IsUnitedNationsMember,
    bool IsG20Member,
    bool IsG7Member,
    GeoCoordinate? Coordinate,
    IReadOnlyList<string> OfficialLanguages,
    IReadOnlyList<string> SpokenLanguages,
    IReadOnlyList<string> Aliases,
    string PostalCodeFormat,
    string TopLevelDomain,
    int SubdivisionCount);

public sealed record AdministrativeDivision(
    string Id,
    string CountryCode,
    string Code,
    string Name,
    string Type,
    string ParentCode,
    GeoCoordinate? Coordinate,
    IReadOnlyList<string> Aliases);

public sealed record CountryOverride(
    string CountryCode,
    string? Name,
    string? OfficialName,
    string? CurrencyCode,
    string? CallingCode,
    string? Notes,
    DateTimeOffset UpdatedAt);

public sealed record SubdivisionOverride(
    string SubdivisionId,
    string? Name,
    string? Type,
    string? Notes,
    DateTimeOffset UpdatedAt);

public sealed record GeographyOverrideSet(
    int SchemaVersion,
    IReadOnlyList<CountryOverride> Countries,
    IReadOnlyList<SubdivisionOverride> Subdivisions)
{
    public static GeographyOverrideSet Empty { get; } = new(1, [], []);
}

public sealed record ReferenceDataRelease(
    string SourceName,
    string SourceUrl,
    string License,
    string SourceCommit,
    string ContentSha256,
    DateTimeOffset LoadedAt);

public sealed record GeographyCatalog(
    IReadOnlyList<Country> Countries,
    IReadOnlyList<AdministrativeDivision> Subdivisions,
    GeographyOverrideSet Overrides,
    ReferenceDataRelease Release);

public enum GeographyValidationSeverity
{
    Information,
    Warning,
    Error,
}

public sealed record GeographyValidationIssue(
    GeographyValidationSeverity Severity,
    string Rule,
    string RecordId,
    string Message);

public sealed record GeographyValidationReport(IReadOnlyList<GeographyValidationIssue> Issues)
{
    public int ErrorCount => this.Issues.Count(static issue => issue.Severity == GeographyValidationSeverity.Error);

    public int WarningCount => this.Issues.Count(static issue => issue.Severity == GeographyValidationSeverity.Warning);

    public bool IsValid => this.ErrorCount == 0;
}

public enum GeographySearchResultKind
{
    Country,
    Subdivision,
}

public sealed record GeographySearchResult(
    GeographySearchResultKind Kind,
    string Id,
    string PrimaryText,
    string SecondaryText,
    double Score);