namespace AtlasOps.Enterprise.Avalonia.Inventory;

using AtlasOps.Enterprise.Avalonia.Common;
using AtlasOps.Enterprise.Contracts.Inventory;
using AtlasOps.Enterprise.Core.Inventory;
using AtlasOps.Enterprise.Core.Scenarios;

using global::Avalonia.Collections;

public sealed class InventoryStudioViewModel : EnterpriseViewModelBase
{
    private readonly DateTimeOffset evaluationTime = DateTimeOffset.UtcNow;
    private readonly AssetMatcher matcher = new();
    private readonly AssetDriftDetector driftDetector = new();
    private readonly InventoryQualityService qualityService = new();
    private readonly IReadOnlyList<AssetRecord> allAssets;
    private AssetRecord? selectedAsset;

    public InventoryStudioViewModel()
    {
        this.allAssets = EnterpriseScenarioCatalog.CreateAssets(this.evaluationTime);
        this.RefreshAssets();
        this.SelectedAsset = this.Assets.FirstOrDefault();
    }

    public override string Title => "Inventory reconciliation studio";

    public override string Summary => "Correlate discovery evidence, explain identity confidence, detect desired-state drift, and score catalog quality.";

    public AvaloniaList<AssetRecord> Assets { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> MatchReasons { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Drift { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Diagnostics { get; } = [];

    public AvaloniaList<EnterpriseMetric> Metrics { get; } = [];

    public AssetRecord? SelectedAsset
    {
        get => this.selectedAsset;
        set
        {
            if (this.selectedAsset == value)
            {
                return;
            }

            this.selectedAsset = value;
            this.OnPropertyChanged();
            this.RefreshSelection();
        }
    }

    public string MatchSummary { get; private set; } = "Select an asset.";

    protected override void OnSearchChanged()
    {
        this.RefreshAssets();
    }

    private void RefreshAssets()
    {
        IEnumerable<AssetRecord> filtered = this.allAssets;
        if (!string.IsNullOrWhiteSpace(this.SearchText))
        {
            filtered = filtered.Where(
                asset =>
                    asset.DisplayName.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    asset.AssetType.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    asset.Owner.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    asset.Identifiers.Any(identifier => identifier.Value.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        this.Assets.Clear();
        this.Assets.AddRange(filtered.OrderBy(static asset => asset.DisplayName, StringComparer.OrdinalIgnoreCase));
        if (this.SelectedAsset is not null && !this.Assets.Contains(this.SelectedAsset))
        {
            this.SelectedAsset = this.Assets.FirstOrDefault();
        }
    }

    private void RefreshSelection()
    {
        this.MatchReasons.Clear();
        this.Drift.Clear();
        this.Diagnostics.Clear();
        this.Metrics.Clear();
        if (this.SelectedAsset is null)
        {
            return;
        }

        AssetEvidence evidence = EnterpriseScenarioCatalog.CreateWebServerEvidence(this.evaluationTime);
        AssetMatchResult match = this.matcher.Match(this.SelectedAsset, evidence);
        this.MatchReasons.AddRange(
            match.Reasons.Select(
                reason => new EnterpriseDetailRow(
                    reason.Kind,
                    reason.Description,
                    reason.Matched ? $"+{reason.Weight:F0}" : $"{reason.Weight:F0}")));

        DesiredAssetState desired = EnterpriseScenarioCatalog.CreateWebServerDesiredState();
        IReadOnlyList<AssetDrift> drift = this.driftDetector.Detect(this.SelectedAsset, desired);
        this.Drift.AddRange(
            drift.Where(static item => item.Kind != DriftKind.Compliant).Select(
                item => new EnterpriseDetailRow(
                    item.Property,
                    item.Explanation,
                    item.Severity.ToString().ToUpperInvariant())));
        InventoryQualityReport quality = this.qualityService.Analyze(this.allAssets, this.evaluationTime, TimeSpan.FromDays(7));
        this.Diagnostics.AddRange(
            quality.Diagnostics.Select(
                diagnostic => new EnterpriseDetailRow("Catalog quality", diagnostic, "REVIEW")));
        this.MatchSummary = match.IsConfidentMatch
            ? $"{match.Score:F0}% confidence: evidence belongs to this asset."
            : $"{match.Score:F0}% confidence: manual review required.";
        this.Metrics.AddRange(
        [
            new("Match confidence", $"{match.Score:F0}%", match.IsConfidentMatch ? "Confident match" : "Needs review"),
            new("Configuration drift", this.Drift.Count.ToString(), $"{drift.Count(static item => item.Kind == DriftKind.Compliant)} compliant"),
            new("Catalog completeness", $"{quality.CompletenessPercent:F0}%", $"{quality.IncompleteAssets} incomplete assets"),
            new("Stale assets", quality.StaleAssets.ToString(), "Not observed within seven days"),
        ]);
        this.OnPropertyChanged(nameof(this.MatchSummary));
    }
}
