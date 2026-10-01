namespace AtlasOps.Features.Mobile.MobileComplianceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileComplianceGovernanceView : UserControl
{
    public MobileComplianceGovernanceView()
    {
        this.DataContext = new MobileComplianceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileComplianceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}