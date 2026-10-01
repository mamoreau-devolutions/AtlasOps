namespace AtlasOps.Features.Storage.ObjectBucketMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObjectBucketMonitoringView : UserControl
{
    public ObjectBucketMonitoringView()
    {
        this.DataContext = new ObjectBucketMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObjectBucketMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}