namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MaintenanceWindowGovernanceView : UserControl
{
    public MaintenanceWindowGovernanceView()
    {
        this.DataContext = new MaintenanceWindowGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MaintenanceWindowGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}