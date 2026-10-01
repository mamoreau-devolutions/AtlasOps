namespace AtlasOps.Geography.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using AtlasOps.Geography.Domain;
using AtlasOps.Geography.Infrastructure;
using AtlasOps.Geography.Services;

using Avalonia.Collections;

public sealed class GeographyCatalogViewModel : INotifyPropertyChanged
{
    private readonly GeographyCatalogService service;
    private string searchText = string.Empty;
    private string statusText = "Geography catalog is not loaded.";
    private bool isBusy;
    private Country? selectedCountry;
    private AdministrativeDivision? selectedSubdivision;
    private string editedCountryName = string.Empty;
    private string editedOfficialName = string.Empty;
    private string editedCurrencyCode = string.Empty;
    private string editedCallingCode = string.Empty;
    private string editedNotes = string.Empty;

    public GeographyCatalogViewModel(GeographyCatalogService service)
    {
        this.service = service;
        this.RefreshCommand = new AsyncCommand(this.InitializeAsync, this.SetError);
        this.SaveCountryCommand = new AsyncCommand(this.SaveCountryAsync, this.SetError);
        this.ResetCountryCommand = new AsyncCommand(this.ResetCountryAsync, this.SetError);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AvaloniaList<Country> Countries { get; } = [];

    public AvaloniaList<AdministrativeDivision> Subdivisions { get; } = [];

    public AvaloniaList<GeographySearchResult> SearchResults { get; } = [];

    public ICommand RefreshCommand { get; }

    public ICommand SaveCountryCommand { get; }

    public ICommand ResetCountryCommand { get; }

    public string SearchText
    {
        get => this.searchText;
        set
        {
            if (!this.SetField(ref this.searchText, value))
            {
                return;
            }

            this.RefreshSearch();
        }
    }

    public string StatusText
    {
        get => this.statusText;
        private set => this.SetField(ref this.statusText, value);
    }

    public bool IsBusy
    {
        get => this.isBusy;
        private set => this.SetField(ref this.isBusy, value);
    }

    public Country? SelectedCountry
    {
        get => this.selectedCountry;
        set
        {
            if (!this.SetField(ref this.selectedCountry, value))
            {
                return;
            }

            this.LoadCountryEditor();
            this.RefreshSubdivisions();
        }
    }

    public AdministrativeDivision? SelectedSubdivision
    {
        get => this.selectedSubdivision;
        set => this.SetField(ref this.selectedSubdivision, value);
    }

    public string EditedCountryName
    {
        get => this.editedCountryName;
        set => this.SetField(ref this.editedCountryName, value);
    }

    public string EditedOfficialName
    {
        get => this.editedOfficialName;
        set => this.SetField(ref this.editedOfficialName, value);
    }

    public string EditedCurrencyCode
    {
        get => this.editedCurrencyCode;
        set => this.SetField(ref this.editedCurrencyCode, value);
    }

    public string EditedCallingCode
    {
        get => this.editedCallingCode;
        set => this.SetField(ref this.editedCallingCode, value);
    }

    public string EditedNotes
    {
        get => this.editedNotes;
        set => this.SetField(ref this.editedNotes, value);
    }

    public string ReleaseSummary { get; private set; } = string.Empty;

    public string ValidationSummary { get; private set; } = string.Empty;

    public static GeographyCatalogViewModel CreateDefault()
    {
        string dataDirectory = Path.Combine(AppContext.BaseDirectory, "ReferenceData", "Geography");
        string applicationDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AtlasOps");
        string overridePath = Path.Combine(applicationDirectory, "geography-overrides.json");
        return new GeographyCatalogViewModel(new GeographyCatalogService(dataDirectory, overridePath));
    }

    public async Task InitializeAsync()
    {
        this.IsBusy = true;
        this.StatusText = "Loading countries and administrative divisions...";
        try
        {
            GeographyCatalog catalog = await this.service.RefreshAsync();
            this.Countries.Clear();
            this.Countries.AddRange(catalog.Countries);
            this.SelectedCountry = this.Countries.FirstOrDefault();
            GeographyValidationReport report = this.service.ValidationReport
                ?? new GeographyValidationReport([]);
            this.ReleaseSummary =
                $"{catalog.Release.SourceName} · {catalog.Release.License} · {catalog.Release.SourceCommit[..Math.Min(12, catalog.Release.SourceCommit.Length)]} · SHA-256 {catalog.Release.ContentSha256[..12]}";
            this.ValidationSummary =
                $"{report.ErrorCount} errors · {report.WarningCount} warnings · {catalog.Overrides.Countries.Count + catalog.Overrides.Subdivisions.Count} overrides";
            this.OnPropertyChanged(nameof(this.ReleaseSummary));
            this.OnPropertyChanged(nameof(this.ValidationSummary));
            this.RefreshSearch();
            this.StatusText =
                $"Loaded {catalog.Countries.Count:N0} countries and {catalog.Subdivisions.Count:N0} administrative divisions.";
        }
        finally
        {
            this.IsBusy = false;
        }
    }

    private async Task SaveCountryAsync()
    {
        Country country = this.SelectedCountry
            ?? throw new InvalidOperationException("Select a country before saving an override.");
        CountryOverride item = new(
            country.Alpha2,
            this.EditedCountryName,
            this.EditedOfficialName,
            this.EditedCurrencyCode,
            this.EditedCallingCode,
            this.EditedNotes,
            DateTimeOffset.UtcNow);
        GeographyCatalog catalog = await this.service.SaveCountryOverrideAsync(item);
        this.ApplyRefreshedCatalog(catalog, country.Alpha2);
        this.StatusText = $"Saved local override for {country.Alpha2}.";
    }

    private async Task ResetCountryAsync()
    {
        Country country = this.SelectedCountry
            ?? throw new InvalidOperationException("Select a country before resetting an override.");
        GeographyCatalog catalog = await this.service.RemoveCountryOverrideAsync(country.Alpha2);
        this.ApplyRefreshedCatalog(catalog, country.Alpha2);
        this.EditedNotes = string.Empty;
        this.StatusText = $"Removed local override for {country.Alpha2}.";
    }

    private void ApplyRefreshedCatalog(GeographyCatalog catalog, string selectedCode)
    {
        this.Countries.Clear();
        this.Countries.AddRange(catalog.Countries);
        this.SelectedCountry = this.Countries.FirstOrDefault(
            country => country.Alpha2.Equals(selectedCode, StringComparison.OrdinalIgnoreCase));
        this.RefreshSearch();
    }

    private void RefreshSearch()
    {
        if (this.service.SearchIndex is null)
        {
            return;
        }

        this.SearchResults.Clear();
        this.SearchResults.AddRange(this.service.Search(this.SearchText));
    }

    private void RefreshSubdivisions()
    {
        this.Subdivisions.Clear();
        if (this.SelectedCountry is null || this.service.Catalog is null)
        {
            return;
        }

        this.Subdivisions.AddRange(
            this.service.Catalog.Subdivisions.Where(
                subdivision => subdivision.CountryCode.Equals(
                    this.SelectedCountry.Alpha2,
                    StringComparison.OrdinalIgnoreCase)));
        this.SelectedSubdivision = this.Subdivisions.FirstOrDefault();
    }

    private void LoadCountryEditor()
    {
        Country? country = this.SelectedCountry;
        this.EditedCountryName = country?.Name ?? string.Empty;
        this.EditedOfficialName = country?.OfficialName ?? string.Empty;
        this.EditedCurrencyCode = country?.CurrencyCode ?? string.Empty;
        this.EditedCallingCode = country?.CallingCode ?? string.Empty;
        CountryOverride? item = this.service.Catalog?.Overrides.Countries.FirstOrDefault(
            value => value.CountryCode.Equals(country?.Alpha2, StringComparison.OrdinalIgnoreCase));
        this.EditedNotes = item?.Notes ?? string.Empty;
    }

    private void SetError(Exception exception)
    {
        this.IsBusy = false;
        this.StatusText = $"Geography operation failed: {exception.Message}";
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        this.OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}