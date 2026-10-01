namespace AtlasOps.Geography.Services;

using AtlasOps.Geography.Data;
using AtlasOps.Geography.Domain;

public sealed class GeographyCatalogService
{
    private readonly string dataDirectory;
    private readonly GeographyDatasetLoader loader;
    private readonly GeographyOverrideStore overrideStore;
    private readonly GeographyCatalogValidator validator;

    public GeographyCatalogService(string dataDirectory, string overrideFilePath)
    {
        this.dataDirectory = dataDirectory;
        this.loader = new GeographyDatasetLoader();
        this.overrideStore = new GeographyOverrideStore(overrideFilePath);
        this.validator = new GeographyCatalogValidator();
    }

    public GeographyCatalog? Catalog { get; private set; }

    public GeographyValidationReport? ValidationReport { get; private set; }

    public GeographySearchIndex? SearchIndex { get; private set; }

    public async Task<GeographyCatalog> RefreshAsync(CancellationToken cancellationToken = default)
    {
        GeographyOverrideSet overrides = await this.overrideStore.LoadAsync(cancellationToken);
        GeographyCatalog catalog = await this.loader.LoadAsync(this.dataDirectory, overrides, cancellationToken);
        this.ValidationReport = this.validator.Validate(catalog);
        if (!this.ValidationReport.IsValid)
        {
            throw new InvalidDataException(
                $"The geography catalog contains {this.ValidationReport.ErrorCount} validation errors.");
        }

        this.Catalog = catalog;
        this.SearchIndex = new GeographySearchIndex(catalog);
        return catalog;
    }

    public IReadOnlyList<GeographySearchResult> Search(string query, int limit = 100)
    {
        GeographySearchIndex index = this.SearchIndex
            ?? throw new InvalidOperationException("The geography catalog has not been loaded.");
        return index.Search(query, limit);
    }

    public async Task<GeographyCatalog> SaveCountryOverrideAsync(
        CountryOverride item,
        CancellationToken cancellationToken = default)
    {
        GeographyOverrideSet current = await this.overrideStore.LoadAsync(cancellationToken);
        List<CountryOverride> countries = current.Countries
            .Where(existing => !existing.CountryCode.Equals(item.CountryCode, StringComparison.OrdinalIgnoreCase))
            .Append(item)
            .OrderBy(static value => value.CountryCode, StringComparer.OrdinalIgnoreCase)
            .ToList();
        await this.overrideStore.SaveAsync(current with { Countries = countries }, cancellationToken);
        return await this.RefreshAsync(cancellationToken);
    }

    public async Task<GeographyCatalog> SaveSubdivisionOverrideAsync(
        SubdivisionOverride item,
        CancellationToken cancellationToken = default)
    {
        GeographyOverrideSet current = await this.overrideStore.LoadAsync(cancellationToken);
        List<SubdivisionOverride> subdivisions = current.Subdivisions
            .Where(existing => !existing.SubdivisionId.Equals(item.SubdivisionId, StringComparison.OrdinalIgnoreCase))
            .Append(item)
            .OrderBy(static value => value.SubdivisionId, StringComparer.OrdinalIgnoreCase)
            .ToList();
        await this.overrideStore.SaveAsync(current with { Subdivisions = subdivisions }, cancellationToken);
        return await this.RefreshAsync(cancellationToken);
    }

    public async Task<GeographyCatalog> RemoveCountryOverrideAsync(
        string countryCode,
        CancellationToken cancellationToken = default)
    {
        GeographyOverrideSet current = await this.overrideStore.LoadAsync(cancellationToken);
        IReadOnlyList<CountryOverride> countries = current.Countries
            .Where(item => !item.CountryCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        await this.overrideStore.SaveAsync(current with { Countries = countries }, cancellationToken);
        return await this.RefreshAsync(cancellationToken);
    }
}