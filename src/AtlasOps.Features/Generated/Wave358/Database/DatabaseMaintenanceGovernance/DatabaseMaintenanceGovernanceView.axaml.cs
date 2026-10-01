namespace AtlasOps.Features.Database.DatabaseMaintenanceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseMaintenanceGovernanceView : UserControl
{
    public DatabaseMaintenanceGovernanceView()
    {
        this.DataContext = new DatabaseMaintenanceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseMaintenanceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}