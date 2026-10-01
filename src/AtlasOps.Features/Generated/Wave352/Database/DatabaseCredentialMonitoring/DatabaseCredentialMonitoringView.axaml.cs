namespace AtlasOps.Features.Database.DatabaseCredentialMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseCredentialMonitoringView : UserControl
{
    public DatabaseCredentialMonitoringView()
    {
        this.DataContext = new DatabaseCredentialMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseCredentialMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}