namespace AtlasOps.Features.Observability.ObservabilityDashboardProvisioning;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class ObservabilityDashboardProvisioningViewModel : INotifyPropertyChanged
{
    private readonly ObservabilityDashboardProvisioningPolicy policy = new();
    private string state = "Draft";
    private string status = "Ready for Observability Dashboard Provisioning operations.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => "Observability Dashboard Provisioning";

    public string Area => "Observability";

    public int Wave => 641;

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