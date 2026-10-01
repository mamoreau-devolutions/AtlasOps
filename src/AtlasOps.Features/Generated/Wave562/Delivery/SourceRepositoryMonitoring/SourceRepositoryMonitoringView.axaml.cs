namespace AtlasOps.Features.Delivery.SourceRepositoryMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SourceRepositoryMonitoringView : UserControl
{
    public SourceRepositoryMonitoringView()
    {
        this.DataContext = new SourceRepositoryMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SourceRepositoryMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}