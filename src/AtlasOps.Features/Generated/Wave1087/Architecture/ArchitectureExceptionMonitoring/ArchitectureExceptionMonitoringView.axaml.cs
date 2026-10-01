namespace AtlasOps.Features.Architecture.ArchitectureExceptionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureExceptionMonitoringView : UserControl
{
    public ArchitectureExceptionMonitoringView()
    {
        this.DataContext = new ArchitectureExceptionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureExceptionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}