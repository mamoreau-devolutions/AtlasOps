namespace AtlasOps.Enterprise.Avalonia.Policy;

using AtlasOps.Enterprise.Avalonia.Common;
using AtlasOps.Enterprise.Contracts.Policy;
using AtlasOps.Enterprise.Core.Policy;
using AtlasOps.Enterprise.Core.Scenarios;

using global::Avalonia.Collections;

public sealed class PolicyStudioViewModel : EnterpriseViewModelBase
{
    private readonly PolicyEvaluator evaluator = new();
    private readonly PolicyAnalyzer analyzer = new();
    private readonly IReadOnlyList<PolicyRequest> allRequests;
    private readonly PolicySet policySet;
    private readonly IReadOnlyList<RoleDefinition> roles;
    private PolicyRequest? selectedRequest;

    public PolicyStudioViewModel()
    {
        this.policySet = EnterpriseScenarioCatalog.CreateOperationsPolicy();
        this.roles = EnterpriseScenarioCatalog.CreateRoles();
        this.allRequests = EnterpriseScenarioCatalog.CreatePolicyRequests();
        this.Rules.AddRange(
            this.policySet.Rules.Select(
                rule => new EnterpriseDetailRow(rule.DisplayName, $"{rule.Effect} · priority {rule.Priority}", rule.Id)));
        this.RefreshRequests();
        this.SelectedRequest = this.Requests.FirstOrDefault();
    }

    public override string Title => "Policy and authorization simulator";

    public override string Summary => "Evaluate role inheritance, attributes, wildcard scopes, priority, and deny precedence with a complete decision trace.";

    public AvaloniaList<PolicyRequest> Requests { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Rules { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Trace { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Diagnostics { get; } = [];

    public AvaloniaList<EnterpriseMetric> Metrics { get; } = [];

    public PolicyRequest? SelectedRequest
    {
        get => this.selectedRequest;
        set
        {
            if (this.selectedRequest == value)
            {
                return;
            }

            this.selectedRequest = value;
            this.OnPropertyChanged();
            this.RefreshDecision();
        }
    }

    public string DecisionSummary { get; private set; } = "Select a request.";

    public string DecisionKind { get; private set; } = PolicyDecisionKind.NotApplicable.ToString();

    protected override void OnSearchChanged()
    {
        this.RefreshRequests();
    }

    private void RefreshRequests()
    {
        IEnumerable<PolicyRequest> filtered = this.allRequests;
        if (!string.IsNullOrWhiteSpace(this.SearchText))
        {
            filtered = filtered.Where(
                request =>
                    request.Subject.Id.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    request.Action.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    request.Resource.Id.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase));
        }

        this.Requests.Clear();
        this.Requests.AddRange(filtered);
        if (this.SelectedRequest is not null && !this.Requests.Contains(this.SelectedRequest))
        {
            this.SelectedRequest = this.Requests.FirstOrDefault();
        }
    }

    private void RefreshDecision()
    {
        this.Trace.Clear();
        this.Diagnostics.Clear();
        this.Metrics.Clear();
        if (this.SelectedRequest is null)
        {
            return;
        }

        PolicyDecision decision = this.evaluator.Evaluate(this.SelectedRequest, this.policySet, this.roles);
        this.DecisionKind = decision.Kind.ToString().ToUpperInvariant();
        this.DecisionSummary = decision.Summary;
        this.Trace.AddRange(
            decision.Trace.Select(
                entry => new EnterpriseDetailRow(
                    entry.RuleName,
                    entry.Explanation,
                    entry.Matched ? $"MATCH · {entry.Effect}" : "SKIP")));
        IReadOnlyList<string> analysis = this.analyzer.Analyze(this.policySet);
        this.Diagnostics.AddRange(
            decision.Diagnostics.Concat(analysis).Select(
                diagnostic => new EnterpriseDetailRow("Policy diagnostic", diagnostic, "REVIEW")));
        this.Metrics.AddRange(
        [
            new("Decision", this.DecisionKind, this.SelectedRequest.Action),
            new("Effective roles", decision.EffectiveRoles.Count.ToString(), string.Join(", ", decision.EffectiveRoles.Order(StringComparer.OrdinalIgnoreCase))),
            new("Rules evaluated", decision.Trace.Count.ToString(), $"{decision.Trace.Count(static item => item.Matched)} matched"),
            new("Policy diagnostics", this.Diagnostics.Count.ToString(), "Static and runtime analysis"),
        ]);
        this.OnPropertyChanged(nameof(this.DecisionKind));
        this.OnPropertyChanged(nameof(this.DecisionSummary));
    }
}
