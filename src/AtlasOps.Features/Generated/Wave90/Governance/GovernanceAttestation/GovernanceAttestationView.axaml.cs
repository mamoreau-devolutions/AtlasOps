namespace AtlasOps.Features.Governance.GovernanceAttestation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class GovernanceAttestationView : UserControl
{
    public GovernanceAttestationView()
    {
        this.DataContext = new GovernanceAttestationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is GovernanceAttestationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}