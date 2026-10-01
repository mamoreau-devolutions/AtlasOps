namespace AtlasOps.Enterprise.Avalonia.Incidents;

using AtlasOps.Enterprise.Avalonia.Common;
using AtlasOps.Enterprise.Contracts.Incidents;
using AtlasOps.Enterprise.Core.Incidents;
using AtlasOps.Enterprise.Core.Scenarios;

using global::Avalonia.Collections;

public sealed class IncidentStudioViewModel : EnterpriseViewModelBase
{
    private readonly DateTimeOffset evaluationTime = DateTimeOffset.UtcNow;
    private readonly IReadOnlyList<Incident> allIncidents;
    private readonly IncidentSlaService slaService;
    private readonly IncidentEscalationService escalationService;
    private readonly ServiceImpactAnalyzer impactAnalyzer = new();
    private Incident? selectedIncident;

    public IncidentStudioViewModel()
    {
        this.allIncidents = EnterpriseScenarioCatalog.CreateIncidents(this.evaluationTime);
        this.slaService = new(IncidentSlaService.CreateDefaultTargets());
        this.escalationService = new(this.slaService);
        this.RefreshIncidents();
        this.SelectedIncident = this.Incidents.FirstOrDefault();
    }

    public override string Title => "Incident command center";

    public override string Summary => "Prioritize active incidents with validated lifecycle, SLA budgets, escalation targets, and dependency-aware impact.";

    public AvaloniaList<Incident> Incidents { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> ImpactedServices { get; } = [];

    public AvaloniaList<EnterpriseMetric> Metrics { get; } = [];

    public Incident? SelectedIncident
    {
        get => this.selectedIncident;
        set
        {
            if (this.selectedIncident == value)
            {
                return;
            }

            this.selectedIncident = value;
            this.OnPropertyChanged();
            this.RefreshSelection();
        }
    }

    public string SlaSummary { get; private set; } = "Select an incident.";

    public string EscalationSummary { get; private set; } = "No escalation evaluated.";

    protected override void OnSearchChanged()
    {
        this.RefreshIncidents();
    }

    private void RefreshIncidents()
    {
        IEnumerable<Incident> filtered = this.allIncidents;
        if (!string.IsNullOrWhiteSpace(this.SearchText))
        {
            filtered = filtered.Where(
                incident =>
                    incident.Id.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    incident.Title.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    incident.ServiceId.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    incident.Owner.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase));
        }

        this.Incidents.Clear();
        this.Incidents.AddRange(filtered.OrderBy(static incident => incident.Severity).ThenBy(static incident => incident.CreatedAt));
        if (this.SelectedIncident is not null && !this.Incidents.Contains(this.SelectedIncident))
        {
            this.SelectedIncident = this.Incidents.FirstOrDefault();
        }
    }

    private void RefreshSelection()
    {
        this.ImpactedServices.Clear();
        this.Metrics.Clear();
        if (this.SelectedIncident is null)
        {
            return;
        }

        IncidentSlaStatus sla = this.slaService.Calculate(this.SelectedIncident, this.evaluationTime);
        IReadOnlyList<OnCallShift> shifts =
        [
            new("platform-primary", "Platform", "Jamie Chen", "Riley Singh", this.evaluationTime.AddHours(-4), this.evaluationTime.AddHours(8)),
        ];
        EscalationRecommendation escalation = this.escalationService.Evaluate(this.SelectedIncident, shifts, this.evaluationTime);
        IReadOnlyList<ServiceImpact> impacts = this.impactAnalyzer.FindImpactedServices(
            this.SelectedIncident.ServiceId,
            EnterpriseScenarioCatalog.CreateServiceDependencies());
        this.ImpactedServices.AddRange(
            impacts.Select(
                impact => new EnterpriseDetailRow(
                    impact.ServiceId,
                    string.Join(" → ", impact.Path),
                    impact.IsCriticalPath ? "CRITICAL PATH" : $"DEPTH {impact.Distance}")));
        this.SlaSummary = sla.IsResolutionBreached
            ? $"Resolution target breached by {-sla.ResolutionRemaining:g}."
            : $"{sla.ResolutionBudgetConsumedPercent:F0}% of resolution budget consumed.";
        this.EscalationSummary = escalation.ShouldEscalate
            ? $"Level {escalation.Level} to {escalation.Target}: {escalation.Reason}"
            : escalation.Reason;
        this.Metrics.AddRange(
        [
            new("Severity", this.SelectedIncident.Severity.ToString().ToUpperInvariant(), this.SelectedIncident.State.ToString()),
            new("Resolution budget", $"{sla.ResolutionBudgetConsumedPercent:F0}%", $"Deadline {sla.ResolutionDeadline:t}"),
            new("Escalation", escalation.Level.ToString(), escalation.Target),
            new("Impacted services", impacts.Count.ToString(), $"{impacts.Count(static item => item.IsCriticalPath)} critical"),
        ]);
        this.OnPropertyChanged(nameof(this.SlaSummary));
        this.OnPropertyChanged(nameof(this.EscalationSummary));
    }
}
