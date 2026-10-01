namespace AtlasOps.Features.Storage.FileShareMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FileShareMonitoringView : UserControl
{
    public FileShareMonitoringView()
    {
        this.DataContext = new FileShareMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FileShareMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}