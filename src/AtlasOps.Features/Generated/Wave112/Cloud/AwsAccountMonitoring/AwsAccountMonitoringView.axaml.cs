namespace AtlasOps.Features.Cloud.AwsAccountMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AwsAccountMonitoringView : UserControl
{
    public AwsAccountMonitoringView()
    {
        this.DataContext = new AwsAccountMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AwsAccountMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}