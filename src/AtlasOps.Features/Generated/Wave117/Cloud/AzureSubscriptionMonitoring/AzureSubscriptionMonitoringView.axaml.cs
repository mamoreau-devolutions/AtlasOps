namespace AtlasOps.Features.Cloud.AzureSubscriptionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AzureSubscriptionMonitoringView : UserControl
{
    public AzureSubscriptionMonitoringView()
    {
        this.DataContext = new AzureSubscriptionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AzureSubscriptionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}