namespace AtlasOps.Features.ServiceManagement.ServiceScorecardGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceScorecardGovernanceView : UserControl
{
    public ServiceScorecardGovernanceView()
    {
        this.DataContext = new ServiceScorecardGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceScorecardGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}