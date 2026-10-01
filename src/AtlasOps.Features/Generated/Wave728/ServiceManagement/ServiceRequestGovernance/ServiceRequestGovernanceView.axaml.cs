namespace AtlasOps.Features.ServiceManagement.ServiceRequestGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceRequestGovernanceView : UserControl
{
    public ServiceRequestGovernanceView()
    {
        this.DataContext = new ServiceRequestGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceRequestGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}