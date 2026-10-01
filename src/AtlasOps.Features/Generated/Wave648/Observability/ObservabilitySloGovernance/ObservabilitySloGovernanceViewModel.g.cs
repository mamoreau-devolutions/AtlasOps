namespace AtlasOps.Features.Observability.ObservabilitySloGovernance;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class ObservabilitySloGovernanceViewModel : INotifyPropertyChanged
{
    private readonly ObservabilitySloGovernancePolicy policy = new();
    private string state = "Draft";
    private string status = "Ready for Observability Slo Governance operations.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => "Observability Slo Governance";

    public string Area => "Observability";

    public int Wave => 648;

    public string State
    {
        get => this.state;
        private set
        {
            if (string.Equals(this.state, value, StringComparison.Ordinal))
            {
                return;
            }

            this.state = value;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.AvailableActions));
        }
    }

    public string Status
    {
        get => this.status;
        private set
        {
            this.status = value;
            this.OnPropertyChanged();
        }
    }

    public IReadOnlyList<string> AvailableActions => this.policy.GetAvailableTransitions(this.State);

    public void Advance()
    {
        string? next = this.AvailableActions.FirstOrDefault();
        if (next is null)
        {
            this.Status = $"{this.Title} is in terminal state {this.State}.";
            return;
        }

        this.State = next;
        this.Status = $"{this.Title} moved to {this.State}.";
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new(propertyName));
    }
}