namespace AtlasOps.Features.ServiceManagement.ProblemRecordProvisioning;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class ProblemRecordProvisioningViewModel : INotifyPropertyChanged
{
    private readonly ProblemRecordProvisioningPolicy policy = new();
    private string state = "Draft";
    private string status = "Ready for Problem Record Provisioning operations.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => "Problem Record Provisioning";

    public string Area => "ServiceManagement";

    public int Wave => 736;

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